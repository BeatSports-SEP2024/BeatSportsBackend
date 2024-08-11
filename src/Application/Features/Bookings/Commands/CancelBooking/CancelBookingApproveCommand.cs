using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Constants;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Hubs;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Bookings.Commands.CancelBooking;
public class CancelBookingApproveCommand : IRequest<BeatSportsResponse>
{
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
}

public class CancelBookingApproveCommandHandler : IRequestHandler<CancelBookingApproveCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IHubContext<BookingHub> _hubContext;
    private readonly IEmailService _emailService;

    public CancelBookingApproveCommandHandler(IBeatSportsDbContext beatSportsDbContext, IHubContext<BookingHub> hubContext, IEmailService emailService)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _hubContext = hubContext;
        _emailService = emailService;
    }

    public async Task<BeatSportsResponse> Handle(CancelBookingApproveCommand request, CancellationToken cancellationToken)
    {
        var bookingApprove = await _beatSportsDbContext.Bookings
            .Include(b => b.CourtSubdivision)
                .ThenInclude(cs => cs.Court)
                    .ThenInclude(c => c.Owner)
                        .ThenInclude(o => o.Account)
                            .ThenInclude(a => a.Wallet)
            .Where(x => x.BookingStatus == BookingEnums.Approved.ToString()
            && x.Id == request.BookingId
            && x.CustomerId == request.CustomerId).FirstOrDefaultAsync();
        if (bookingApprove == null)
        {
            throw new NotFoundException("Not Found");
        }

        // Chuyển đổi Unix timestamp ngược lại thành DateTime
        // DateTimeOffset dateTimeOffsetFromUnix = DateTimeOffset.FromUnixTimeSeconds(bookingApprove.UnixTimestampMinCancellation);
        DateTime datetimeFromUnix = bookingApprove.UnixTimestampMinCancellation;
        // Ghép PlayingDate và StartTimePlaying lại
        DateTime playingStartDateTime = bookingApprove.PlayingDate.Date.Add(bookingApprove.StartTimePlaying);

        // Lấy thời gian hiện tại
        /*        DateTime currentDateTime = DateTime.Now;

                // So sánh thời gian hủy với thời gian hiện tại
                TimeSpan timeDifference = playingStartDateTime - currentDateTime;*/
        // Lấy thời gian sẽ chơi trừ cho thời gian min có thể hủy, nếu nó nhỏ hơn 0 thì không thể hủy được do đã vô khung kh thể hủy
        TimeSpan timeDifference = DateTime.Now - datetimeFromUnix;
        if (timeDifference >= TimeSpan.Zero)
        {
            // Thời gian hủy nhỏ hơn hoặc bằng thời gian hiện tại
            throw new BadRequestException($"Không thể hủy đặt sân, thời gian tối thiểu để hủy lịch đặt sân đã trôi qua. Thời gian bị lệch: {timeDifference}");
        }
        DateTime startTime = bookingApprove.PlayingDate.Date.Add(bookingApprove.StartTimePlaying);
        DateTime endTime = bookingApprove.PlayingDate.Date.Add(bookingApprove.EndTimePlaying);
        var timeChecking = _beatSportsDbContext.TimeChecking
                           .Where(x => x.CourtSubdivisionId == bookingApprove.CourtSubdivisionId
                           && x.StartTime == startTime && x.EndTime == endTime && x.DateBooking == bookingApprove.PlayingDate)
                           .FirstOrDefault();
        if (timeChecking != null)
        {
            _beatSportsDbContext.TimeChecking.Remove(timeChecking);
            //_beatSportsDbContext.SaveChanges();
        }

        // check campaign
        if (bookingApprove.CampaignId != null)
        {
            var campaignExist = await _beatSportsDbContext.Campaigns.Where(c => c.Id == bookingApprove.CampaignId).FirstOrDefaultAsync();
            campaignExist!.QuantityOfCampaign += 1;
            _beatSportsDbContext.Campaigns.Update(campaignExist);
        }
        var ownerWallet = bookingApprove.CourtSubdivision.Court.Owner.Account.Wallet;

        var getCustomerByAccount = _beatSportsDbContext.Customers
            .Include(c => c.Account)
            .Where(x => x.Id == request.CustomerId && !x.IsDelete).SingleOrDefault();

        if (getCustomerByAccount == null)
        {
            throw new NotFoundException("Cannot find this customer");
        }

        var accountId = getCustomerByAccount.AccountId;

        var customerWallet = _beatSportsDbContext.Wallets
            .Where(x => x.AccountId == accountId && !x.IsDelete).SingleOrDefault();

        // Nếu ví có tồn tại
        if (customerWallet != null)
        {
            // Nếu booking đó có transaction
            if (bookingApprove.TransactionId.HasValue)
            {
                var transaction = await _beatSportsDbContext.Transactions
                    .Where(t => t.Id == bookingApprove.TransactionId.Value && !t.IsDelete)
                    .FirstOrDefaultAsync();
                // Nếu transaction không null, mà thực chất ID thì làm gì kh có transaction được
                if (transaction != null)
                {
                    // Cộng lại tiền cho ví
                    // Decimal trong trường hợp này kh có null được đâu. Khỏi lo
                    customerWallet.Balance += (decimal)transaction.TransactionAmount!;
                    _beatSportsDbContext.Wallets.Update(customerWallet);
                    // Trừ tiền tương đương trong ví owner với số tiền customer đặt đơn
                    ownerWallet.Balance -= (decimal)transaction.TransactionAmount!;
                    _beatSportsDbContext.Wallets.Update(ownerWallet);
                    // Hủy transaction đó
                    transaction.TransactionMessage = TransactionConstant.TransactionCancel;
                    transaction.TransactionStatus = TransactionEnum.Cancel.ToString();
                    transaction.AdminCheckStatus = AdminCheckEnums.Cancel;
                    _beatSportsDbContext.Transactions.Update(transaction);
                }
            }
        }
        // Hủy booking đó
        bookingApprove.BookingStatus = BookingEnums.Cancel.ToString();
        _beatSportsDbContext.Bookings.Update(bookingApprove);
        await _beatSportsDbContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("DeleteBooking");
        if(getCustomerByAccount.Account.Email != null)
        {
            await _emailService.SendEmailAsync(
                    getCustomerByAccount.Account.Email,
                    "Thông báo hủy đơn đặt sân",
                    $@"
                            <html>
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
                                    }}
                                    .content p {{
                                        margin: 10px 0;
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
                                        <p>Bạn đã hủy sân đã thành công, tiền đã được hoàn về ví</p>
                                        <p><strong>Mã đơn đặt sân: </strong> {bookingApprove.Id}</p>
                                        <p><strong>Tên sân:</strong> {bookingApprove.CourtSubdivision.Court.CourtName}</p>
                                        <p><strong>Sân con:</strong> {bookingApprove.CourtSubdivision.CourtSubdivisionName}</p>
                                        <p><strong>Thời gian bắt đầu:</strong> {bookingApprove.StartTimePlaying}</p>
                                        <p><strong>Thời gian kết thúc:</strong> {bookingApprove.EndTimePlaying}</p>
                                        <p><strong>Tổng số tiền đã thanh toán:</strong> {bookingApprove.TotalAmount} VND</p>
                                        <p><strong>Số tiền đã được giảm giá từ voucher:</strong> {bookingApprove.TotalPriceDiscountCampaign} VND</p>
                                        <p><strong>Trạng thái:</strong> {bookingApprove.BookingStatus}</p>
                                        <p><strong>Số tiền được hoàn lại:</strong> {bookingApprove.TotalAmount}</p>
                                    </div>
                                    <div class='footer'>
                                        <p>© 2024 BeatSports. All rights reserved.</p>
                                    </div>
                                </div>
                            </body>
                            </html>"
                );
        }


        return new BeatSportsResponse
        {
            Message = "Cancel Booking Successfully"
        };
    }
}
