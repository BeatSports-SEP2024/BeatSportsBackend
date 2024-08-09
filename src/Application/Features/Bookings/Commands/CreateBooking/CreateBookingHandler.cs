using BeatSportsAPI.Application.Common.Constants;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Cloudinaries;
using BeatSportsAPI.Application.Features.Hubs;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using Services.Redis;
using StackExchange.Redis;

namespace BeatSportsAPI.Application.Features.Bookings.Commands.CreateBooking;
public class CreateBookingHandler : IRequestHandler<CreateBookingCommand, BookingSuccessResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IDatabase _database;
    private readonly IHubContext<BookingHub> _hubContext;
    private readonly IMediator _mediator;
    private readonly IEmailService _emailService;

    public CreateBookingHandler(IBeatSportsDbContext beatSportsDbContext, IMediator mediator, IDatabase database, IHubContext<BookingHub> hubContext, IEmailService emailService)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mediator = mediator;
        _database = database;
        _hubContext = hubContext;
        _emailService = emailService;
    }
    private async Task<string> CreateAndUploadQRCode(string bookingId)
    {
        // Tạo QR code
        using var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(bookingId, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeBytes = qrCode.GetGraphic(20);

        // Chuyển QR code bytes thành Stream
        await using var qrStream = new MemoryStream(qrCodeBytes);

        // Tạo ImageUploadCommand và gửi qua MediatR
        var command = new ImageUploadCommand
        {
            ImageStream = qrStream,
            FileName = $"booking_{bookingId}_qr.png"
        };

        var imageUrl = await _mediator.Send(command);
        return imageUrl;
    }
    public async Task<BookingSuccessResponse> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {

        var checkBookingInDB = await _beatSportsDbContext.Bookings
            .Include(b => b.CourtSubdivision)
                .ThenInclude(cs => cs.Court)
                    .ThenInclude(c => c.Owner)
                        .ThenInclude(o => o.Account)
                            .ThenInclude(a => a.Wallet)
            .Where(x => x.Id == request.BookingId && x.BookingStatus == BookingEnums.Process.ToString() && !x.IsDelete)
            .SingleOrDefaultAsync();
        if (checkBookingInDB == null)
        {
            throw new NotFoundException("Không tìm thấy booking như trên");
        }
        // Check account dựa trên booking của customer đó
        var getCustomerByAccount = _beatSportsDbContext.Customers
            .Include(c => c.Account)
                .Where(x => x.Id == checkBookingInDB.CustomerId && !x.IsDelete).SingleOrDefault();

        // Check account cua owner dua tren booking 
        if (checkBookingInDB.CourtSubdivision.Court.Owner.Account.Wallet == null)
        {
            throw new BadRequestException("Owner wallet not found");
        }

        var ownerWallet = checkBookingInDB.CourtSubdivision.Court.Owner.Account.Wallet;

        if (getCustomerByAccount == null)
        {
            throw new NotFoundException("Cannot find this customer");
        }

        var accountId = getCustomerByAccount.AccountId;

        var customerWallet = _beatSportsDbContext.Wallets
            .Where(x => x.AccountId == accountId && !x.IsDelete).SingleOrDefault();

        #region Redis
        //tạo key trong Redis
        string lockKey = $"booking:{checkBookingInDB.CourtSubdivisionId}:{checkBookingInDB.StartTimePlaying}:lock";
        //tạo value tương ứng với key vừa tạo trong Redis
        string lockValue = Guid.NewGuid().ToString();
        //thời gian hết hạn
        TimeSpan expiry = TimeSpan.FromSeconds(30);
        #endregion

        using (var redisLock = new RedisLock(_database, lockKey, lockValue, expiry))
        {
            if (redisLock.AcquireLock())
            {
                try
                {
                    // Case check  
                    // Check lại giá tổng coi đã đúng hay chưa.
                    if (request.CampaignId != null)
                    {
                        var campaignIfExist = await _beatSportsDbContext.Campaigns.Where(x => x.Id == request.CampaignId
                        && x.QuantityOfCampaign > 0
                        && x.EndDateApplying >= checkBookingInDB.PlayingDate
                        && !x.IsDelete).SingleOrDefaultAsync();
                        if (campaignIfExist == null)
                        {
                            throw new BadRequestException("Campaign đã hết số lượng sử dụng hoặc hết hạn sử dụng");
                        }
                        // Nếu có tồn tại thì check lại giá tiền
                        // Đối với booking status process được lưu trong db thì giá đang là giá ch apply campaign
                        // Tính giá trị giảm trước
                        var value = (checkBookingInDB.TotalAmount * campaignIfExist.PercentDiscount) / 100;
                        decimal realApply = 0;
                        if (value > campaignIfExist.MaxValueDiscount)
                        {
                            realApply = campaignIfExist.MaxValueDiscount;
                        }
                        else
                        {
                            realApply = value;
                        }
                        var checkTotalMoney = checkBookingInDB.TotalAmount - realApply;
                        if (checkTotalMoney != request.TotalAmount)
                        {
                            throw new BadRequestException("Tiền không đúng với giá trị thực tế khi áp dụng mã giảm giá");
                        }

                        // Xét 2 trường hợp
                        if (customerWallet != null)
                        {
                            if (customerWallet.Balance >= checkTotalMoney)
                            {
                                customerWallet.Balance -= checkTotalMoney;
                                _beatSportsDbContext.Wallets.Update(customerWallet);
                                var transaction = new Transaction
                                {
                                    WalletId = customerWallet.Id,
                                    WalletTargetId = ownerWallet.Id,
                                    TransactionMessage = TransactionConstant.TransactionSuccessMessage,
                                    TransactionPayload = "",
                                    TransactionStatus = TransactionEnum.Pending.ToString(),
                                    AdminCheckStatus = 0,
                                    TransactionAmount = checkTotalMoney,
                                    TransactionDate = DateTime.UtcNow,
                                    TransactionType = TransactionConstant.TransactionTypeInApp,
                                    IsDelete = false
                                };
                                _beatSportsDbContext.Transactions.Add(transaction);
                                checkBookingInDB.TransactionId = transaction.Id;

                                // => Sau khi booking xong, sẽ ghi trạng thái transaction là pending
                                // Sau khi admin check chỗ tiền này chuyển cho owner, đổi thành approved
                            }
                            else if (customerWallet.Balance < checkTotalMoney)
                            {
                                ///// TODO: cần check lại là có cần thiết phải hủy nếu kh đủ tiền kh
                                throw new BadRequestException("Balance is not enough for booking");
                            }
                        }

                        //Update Campaign to Booking
                        checkBookingInDB.CampaignId = request.CampaignId;
                        checkBookingInDB.TotalPriceDiscountCampaign = realApply;
                        checkBookingInDB.BookingStatus = BookingEnums.Approved.ToString();
                        //Giảm giá tiền sau khi áp dụng campaign
                        checkBookingInDB.TotalAmount = checkTotalMoney;
                        //Cập nhật số lượng campaign
                        campaignIfExist.QuantityOfCampaign = campaignIfExist.QuantityOfCampaign - 1;
                        _beatSportsDbContext.Campaigns.Update(campaignIfExist);
                        _beatSportsDbContext.Bookings.Update(checkBookingInDB);
                        await _beatSportsDbContext.SaveChangesAsync();
                        Console.WriteLine($"Booking {checkBookingInDB.CustomerId} is complete.");

                        // tạo hình ảnh 
                        var qrCodeUrl = await CreateAndUploadQRCode(checkBookingInDB.Id.ToString());

                        await _emailService.SendEmailAsync(
                            getCustomerByAccount.Account.Email,
                            "Hóa đơn đặt sân chi tiết",
                                                   $@"<html>
<head>
    <style>
        body {{
            font-family: Montserrat, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f4f4f4;
        }}
        .container {{
            width: 100%;
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            padding: 20px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            background-color: #007bff;
            color: #ffffff;
            padding: 10px 0;
            text-align: center;
            font-size: 24px;
        }}
        .content {{
            margin: 20px 0;
            line-height: 1.6;
            text-align: center;
        }}
        .content img {{
            max-width: 50%; /* Giới hạn kích thước tối đa của ảnh là 50% chiều rộng của container */
            height: auto; /* Đảm bảo tỷ lệ chiều cao phù hợp */
            margin-top: 10px;
            display: block;
            margin-left: auto;
            margin-right: auto;
        }}
        .footer {{
            margin: 20px 0;
            text-align: center;
            color: #777;
            font-size: 12px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            Thông tin tài khoản chủ sân
        </div>
        <div class='content'>
            <p>Kính gửi {getCustomerByAccount.Account.FirstName + " " + getCustomerByAccount.Account.LastName},</p>
            <p>Chúng tôi xin thông báo rằng việc đặt phòng của bạn đã thành công với các thông tin sau:</p>
            <p><strong>Mã đặt phòng:</strong> {checkBookingInDB.Id}</p>
            <p><strong>Tên sân:</strong> {checkBookingInDB.CourtSubdivision.Court.CourtName}</p>
            <p><strong>Sân con:</strong> {checkBookingInDB.CourtSubdivision.CourtSubdivisionName}</p>
            <p><strong>Thời gian bắt đầu:</strong> {checkBookingInDB.StartTimePlaying}</p>
            <p><strong>Thời gian kết thúc:</strong> {checkBookingInDB.EndTimePlaying}</p>
            <p><strong>Tổng số tiền thanh toán:</strong> {checkBookingInDB.TotalAmount} VND</p>
            <p><strong>Số tiền đã giảm giá từ voucher:</strong> {checkBookingInDB.TotalPriceDiscountCampaign} VND</p>
            <p><strong>Trạng thái:</strong> {checkBookingInDB.BookingStatus}</p>
            <p><strong>Thông tin chủ sân:</strong> {checkBookingInDB.CourtSubdivision.Court.Owner.Account.FirstName} {checkBookingInDB.CourtSubdivision.Court.Owner.Account.LastName}</p>
            <img src='{qrCodeUrl}' alt='QR Code for your booking' />
            <p>Vui lòng đến sân và check-in trong vòng 30 phút kể từ thời gian bắt đầu. Nếu không check-in, đặt sân sẽ bị hủy và không hoàn trả tiền.</p>
        </div>
        <div class='footer'>
            <p>© 2024 BeatSports. All rights reserved.</p>
        </div>
    </div>
</body>
</html>
"
                        );
                        return new BookingSuccessResponse
                        {
                            BookingId = checkBookingInDB.Id,
                            Message = "Booking Successfully"
                        };
                    }

                    // Case không áp dụng campaign
                    else
                    {

                        // Xét 2 trường hợp
                        if (customerWallet != null)
                        {
                            if (customerWallet.Balance >= checkBookingInDB.TotalAmount)
                            {
                                customerWallet.Balance -= checkBookingInDB.TotalAmount;
                                _beatSportsDbContext.Wallets.Update(customerWallet);
                                var transaction = new Transaction
                                {
                                    WalletId = customerWallet.Id,
                                    WalletTargetId = ownerWallet.Id,
                                    TransactionMessage = TransactionConstant.TransactionSuccessMessage,
                                    TransactionPayload = null,
                                    TransactionStatus = TransactionEnum.Pending.ToString(),
                                    AdminCheckStatus = 0,
                                    TransactionAmount = checkBookingInDB.TotalAmount,
                                    TransactionDate = DateTime.UtcNow,
                                    TransactionType = TransactionConstant.TransactionTypeInApp,
                                    IsDelete = false
                                };
                                _beatSportsDbContext.Transactions.Add(transaction);
                                checkBookingInDB.TransactionId = transaction.Id;
                                // => Sau khi booking xong, sẽ ghi trạng thái transaction là pending
                                // Sau khi admin check chỗ tiền này chuyển cho owner, đổi thành approved
                            }
                            else if (customerWallet.Balance < checkBookingInDB.TotalAmount)
                            {
                                ///// TODO: cần check lại là có cần thiết phải hủy nếu kh đủ tiền kh
                                //checkBookingInDB.BookingStatus = BookingEnums.Cancel.ToString();
                                //_beatSportsDbContext.Bookings.Update(checkBookingInDB);

                                //var transaction = new Transaction
                                //{
                                //    WalletId = customerWallet.Id,
                                //    WalletTargetId = ownerWallet.Id,
                                //    TransactionMessage = TransactionConstant.TransactionFailedInsufficientBalance,
                                //    TransactionPayload = null,
                                //    TransactionStatus = TransactionEnum.Cancel.ToString(),
                                //    AdminCheckStatus = 0,
                                //    TransactionAmount = checkBookingInDB.TotalAmount,
                                //    TransactionDate = DateTime.UtcNow,
                                //    TransactionType = TransactionConstant.TransactionTypeInApp,
                                //    IsDelete = false
                                //};
                                //_beatSportsDbContext.Transactions.Add(transaction);
                                //checkBookingInDB.TransactionId = transaction.Id;
                                //await _beatSportsDbContext.SaveChangesAsync();
                                throw new BadRequestException("Balance is not enough for booking");
                            }
                        }
                        checkBookingInDB.BookingStatus = BookingEnums.Approved.ToString();
                        _beatSportsDbContext.Bookings.Update(checkBookingInDB);
                        await _beatSportsDbContext.SaveChangesAsync();
                        Console.WriteLine($"Booking {checkBookingInDB.CustomerId} is complete.");
                        var qrCodeUrl = await CreateAndUploadQRCode(checkBookingInDB.Id.ToString());

                        await _emailService.SendEmailAsync(
                            getCustomerByAccount.Account.Email,
                            "Hóa đơn đặt sân chi tiết",
                                                   $@"<html>
<head>
    <style>
        body {{
            font-family: Montserrat, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f4f4f4;
        }}
        .container {{
            width: 100%;
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            padding: 20px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            background-color: #007bff;
            color: #ffffff;
            padding: 10px 0;
            text-align: center;
            font-size: 24px;
        }}
        .content {{
            margin: 20px 0;
            line-height: 1.6;
            text-align: center;
        }}
        .content img {{
            max-width: 50%; /* Giới hạn kích thước tối đa của ảnh là 50% chiều rộng của container */
            height: auto; /* Đảm bảo tỷ lệ chiều cao phù hợp */
            margin-top: 10px;
            display: block;
            margin-left: auto;
            margin-right: auto;
        }}
        .footer {{
            margin: 20px 0;
            text-align: center;
            color: #777;
            font-size: 12px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            Thông tin tài khoản chủ sân
        </div>
        <div class='content'>
            <p>Kính gửi {getCustomerByAccount.Account.FirstName + " " + getCustomerByAccount.Account.LastName},</p>
            <p>Chúng tôi xin thông báo rằng việc đặt phòng của bạn đã thành công với các thông tin sau:</p>
            <p><strong>Mã đặt phòng:</strong> {checkBookingInDB.Id}</p>
            <p><strong>Tên sân:</strong> {checkBookingInDB.CourtSubdivision.Court.CourtName}</p>
            <p><strong>Sân con:</strong> {checkBookingInDB.CourtSubdivision.CourtSubdivisionName}</p>
            <p><strong>Thời gian bắt đầu:</strong> {checkBookingInDB.StartTimePlaying}</p>
            <p><strong>Thời gian kết thúc:</strong> {checkBookingInDB.EndTimePlaying}</p>
            <p><strong>Tổng số tiền thanh toán:</strong> {checkBookingInDB.TotalAmount} VND</p>
            <p><strong>Số tiền đã giảm giá từ voucher:</strong> {checkBookingInDB.TotalPriceDiscountCampaign} VND</p>
            <p><strong>Trạng thái:</strong> {checkBookingInDB.BookingStatus}</p>
            <p><strong>Thông tin chủ sân:</strong> {checkBookingInDB.CourtSubdivision.Court.Owner.Account.FirstName} {checkBookingInDB.CourtSubdivision.Court.Owner.Account.LastName}</p>
            <img src='{qrCodeUrl}' alt='QR Code for your booking' />
            <p>Vui lòng đến sân và check-in trong vòng 30 phút kể từ thời gian bắt đầu. Nếu không check-in, đặt sân sẽ bị hủy và không hoàn trả tiền.</p>
        </div>
        <div class='footer'>
            <p>© 2024 BeatSports. All rights reserved.</p>
        </div>
    </div>
</body>
</html>
"
                        );

                        return new BookingSuccessResponse
                        {
                            BookingId = checkBookingInDB.Id,
                            Message = "Booking Successfully"
                        };
                    }
                }
                finally
                {
                    await _hubContext.Clients.All.SendAsync("CreateBooking");
                    redisLock.ReleaseLock();
                }
            }
            else
            {
                //test console
                Console.WriteLine($"Booking in {request.CourtSubdivisionId} is already in progress by another instance.");
                return new BookingSuccessResponse
                {
                    Message = $"Booking in {request.CourtSubdivisionId} is already in progress by another instance."
                };
            }
        }
    }
}