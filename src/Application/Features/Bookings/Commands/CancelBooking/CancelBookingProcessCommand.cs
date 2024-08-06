using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BeatSportsAPI.Application.Common.Constants;
using BeatSportsAPI.Application.Features.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace BeatSportsAPI.Application.Features.Bookings.Commands.CancelBooking;
public class CancelBookingProcessCommand : IRequest<BeatSportsResponse>
{
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
}

public class CancelBookingProcessCommandHandler : IRequestHandler<CancelBookingProcessCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IHubContext<BookingHub> _hubContext;
    private readonly IEmailService _emailService;

    public CancelBookingProcessCommandHandler(IBeatSportsDbContext beatSportsDbContext, IHubContext<BookingHub> hubContext, IEmailService emailService)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _hubContext = hubContext;
        _emailService = emailService;
    }

    public async Task<BeatSportsResponse> Handle(CancelBookingProcessCommand request, CancellationToken cancellationToken)
    {
        var bookingProcess = await _beatSportsDbContext.Bookings
            .Where(x => x.BookingStatus == BookingEnums.Process.ToString() 
            && x.Id == request.BookingId 
            && x.CustomerId == request.CustomerId).FirstOrDefaultAsync();
        if (bookingProcess == null)
        {
            throw new NotFoundException("Not Found");
        }

        DateTime startTime = bookingProcess.PlayingDate.Date.Add(bookingProcess.StartTimePlaying);
        DateTime endTime = bookingProcess.PlayingDate.Date.Add(bookingProcess.EndTimePlaying);
        var timeChecking = _beatSportsDbContext.TimeChecking
                           .Where(x => x.CourtSubdivisionId == bookingProcess.CourtSubdivisionId
                           && x.StartTime == startTime && x.EndTime == endTime && x.DateBooking == bookingProcess.PlayingDate)
                           .FirstOrDefault();
        if (timeChecking != null)
        {
            var getCustomerByAccount = _beatSportsDbContext.Customers
            .Where(x => x.Id == request.CustomerId && !x.IsDelete).SingleOrDefault();

            if (getCustomerByAccount == null)
            {
                throw new NotFoundException("Cannot find this customer");
            }

            var accountId = getCustomerByAccount.AccountId;

            var customerWallet = _beatSportsDbContext.Wallets
                .Where(x => x.AccountId == accountId && !x.IsDelete).SingleOrDefault();

            if (customerWallet != null)
            {
                customerWallet.Balance += bookingProcess.TotalAmount;
                _beatSportsDbContext.Wallets.Update(customerWallet);
            }

            if (bookingProcess.TransactionId.HasValue)
            {
                var transaction = await _beatSportsDbContext.Transactions
                    .Where(t => t.Id == bookingProcess.TransactionId.Value && !t.IsDelete)
                    .FirstOrDefaultAsync();

                if (transaction != null)
                {
                    transaction.TransactionMessage = TransactionConstant.TransactionCancel;
                    transaction.TransactionStatus = TransactionEnum.Cancel.ToString();
                    _beatSportsDbContext.Transactions.Update(transaction);
                }
            }

            _beatSportsDbContext.TimeChecking.Remove(timeChecking);
            _beatSportsDbContext.Bookings.Remove(bookingProcess);
            _beatSportsDbContext.SaveChanges();
            await _hubContext.Clients.All.SendAsync("DeleteBookingProcess");
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
                                        <p><strong>Mã đơn đặt sân: </strong> {bookingProcess.Id}</p>
                                        <p><strong>Tên sân:</strong> {bookingProcess.CourtSubdivision.Court.CourtName}</p>
                                        <p><strong>Sân con:</strong> {bookingProcess.CourtSubdivision.CourtSubdivisionName}</p>
                                        <p><strong>Thời gian bắt đầu:</strong> {bookingProcess.StartTimePlaying}</p>
                                        <p><strong>Thời gian kết thúc:</strong> {bookingProcess.EndTimePlaying}</p>
                                        <p><strong>Tổng số tiền đã thanh toán:</strong> {bookingProcess.TotalAmount} VND</p>
                                        <p><strong>Số tiền đã được giảm giá từ voucher:</strong> {bookingProcess.TotalPriceDiscountCampaign} VND</p>
                                        <p><strong>Trạng thái:</strong> {bookingProcess.BookingStatus}</p>
                                        <p><strong>Số tiền được hoàn lại:</strong> {bookingProcess.TotalAmount}</p>
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
