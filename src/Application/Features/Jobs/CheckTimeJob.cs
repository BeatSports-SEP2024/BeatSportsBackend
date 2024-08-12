using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Jobs;
public class CheckTimeJob
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IEmailService _emailService;

    public CheckTimeJob(IBeatSportsDbContext beatSportsDbContext, IEmailService emailService)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _emailService = emailService;
    }
    public void CheckTimeOfCourt()
    {
        var bookingList = _beatSportsDbContext.Bookings.Where(x => x.BookingStatus == BookingEnums.Process.ToString()).ToList();
        foreach (var booking in bookingList)
        {

            var checkAfter2Minutes = booking.Created.AddMinutes(2);
            if (DateTime.Now > checkAfter2Minutes)
            {
                DateTime startTime = booking.PlayingDate.Date.Add(booking.StartTimePlaying);
                DateTime endTime = booking.PlayingDate.Date.Add(booking.EndTimePlaying);
                var timeChecking = _beatSportsDbContext.TimeChecking
                                   .Where(x => x.CourtSubdivisionId == booking.CourtSubdivisionId
                                   && x.StartTime == startTime && x.EndTime == endTime && x.DateBooking == booking.PlayingDate)
                                   .FirstOrDefault();


                if (timeChecking != null)
                {
                    timeChecking.IsDelete = true;
                    _beatSportsDbContext.TimeChecking.Remove(timeChecking);
                    _beatSportsDbContext.SaveChanges();

                    booking.IsDelete = true;
                    _beatSportsDbContext.Bookings.Remove(booking);
                    _beatSportsDbContext.SaveChanges();

                    Console.WriteLine($"Recurring job executed start for Booking ${booking.Id}");
                }
            }
        }
    }

    public void CheckTimeOfBooking()
    {
        // Lấy những booking nào đang có status là Approved ra 
        var bookingList = _beatSportsDbContext.Bookings
                        .Where(x => !x.IsDelete && x.BookingStatus == BookingEnums.Approved.ToString())
                        .ToList();

        foreach (var booking in bookingList)
        {
            // UnixTimestampMinCancellation là thời gian tối thiểu để hủy booking (này bao gồm cả ngày và giờ rồi)
            DateTime datetimeFromUnix = booking.UnixTimestampMinCancellation;
            // Ghép PlayingDate và StartTimePlaying lại
            DateTime playingStartDateTime = booking.PlayingDate.Date.Add(booking.StartTimePlaying);

            // So sánh thời gian hủy với thời gian hiện tại
            TimeSpan timeDifference = DateTime.Now - datetimeFromUnix;

            if (timeDifference >= TimeSpan.Zero)
            {
                // Thời gian hủy nhỏ hơn hoặc bằng thời gian hiện tại
                //throw new BadRequestException($"Không thể hủy đặt sân, thời gian tối thiểu để hủy lịch đặt sân đã trôi qua. Thời gian bị lệch: {timeDifference}");
                var transaction = _beatSportsDbContext.Transactions
                                .Where(x => x.Id == booking.TransactionId && x.AdminCheckStatus == AdminCheckEnums.Pending)
                                .FirstOrDefault();
                if (transaction != null)
                {
                    var ownerWallet = _beatSportsDbContext.Wallets
                                .Where(x => x.Id == transaction.WalletTargetId)
                                .FirstOrDefault();
                    // Không hiểu nổi 
                    transaction.AdminCheckStatus = AdminCheckEnums.Accepted;
                    // Hoàn thành transaction này để đánh dấu nó approve
                    transaction.TransactionStatus = TransactionEnum.Approved.ToString();
                    // Cộng vào ví owner
                    ownerWallet.Balance += (int)transaction.TransactionAmount;

                    _beatSportsDbContext.Transactions.Update(transaction);
                    _beatSportsDbContext.Wallets.Update(ownerWallet);
                    _beatSportsDbContext.SaveChanges();
                }

            }
        }
    }

    public void CheckBookingPlayDateIfFinish()
    {
        var bookingList = _beatSportsDbContext.Bookings
            .Where(x => !x.IsDelete && x.BookingStatus == BookingEnums.Approved.ToString())
            .ToList();

        foreach (var booking in bookingList)
        {
            var endDatePlaying = booking.PlayingDate.Add(booking.EndTimePlaying);
            if (endDatePlaying <= DateTime.Now)
            {
                booking.BookingStatus = BookingEnums.Finished.ToString();

                // Tạo thông báo feedback
                var customer = _beatSportsDbContext.Customers.Where(a => a.Id == booking.CustomerId).SingleOrDefault();
                var notification = _beatSportsDbContext.Notifications.Where(n => n.BookingId == booking.Id.ToString()).SingleOrDefault();
                if (notification != null)
                {
                    notification.Title = "Đánh giá sân";
                    notification.Message = $"hãy để lại phản hồi cho sân {booking.CourtSubdivision.Court.CourtName} mà bạn đã chơi.";
                    notification.IsRead = false;
                    notification.Type = "Feedback";

                    _beatSportsDbContext.Notifications.Update(notification);
                }
            }
        }
        _beatSportsDbContext.SaveChanges();
    }

    public void RemoveRoomWhenExpired()
    {
        var expiredRooms = _beatSportsDbContext.RoomMatches
            .Where(x => !x.IsDelete && x.EndTimeRoom <= DateTime.Now).ToList();

        foreach (var room in expiredRooms)
        {
            // Lấy và xóa tất cả RoomMembers liên quan đến phòng hết hạn
            var roomMembers = _beatSportsDbContext.RoomMembers
                .Where(rm => rm.RoomMatchId == room.Id).ToList();
            _beatSportsDbContext.RoomMembers.RemoveRange(roomMembers);

            // Lấy và xóa tất cả RoomJoiningRequests liên quan đến phòng hết hạn
            var roomJoiningRequests = _beatSportsDbContext.RoomRequests
                .Where(rj => rj.RoomMatchId == room.Id).ToList();
            _beatSportsDbContext.RoomRequests.RemoveRange(roomJoiningRequests);

            // Xóa chính phòng đã hết hạn
            _beatSportsDbContext.RoomMatches.Remove(room);
        }
        _beatSportsDbContext.SaveChanges();
    }

    public void NotificationForOwnerPayFee()
    {
        var today = DateTime.Now;
        if (today.Day == 10)
        {
            var owners = _beatSportsDbContext.Owners.ToList();
            foreach (var owner in owners)
            {
                var notification = new Notification
                {
                    AccountId = owner.AccountId,
                    Title = "Thanh toán phí dịch vụ",
                    Message = "đã đến ngày 10, vui lòng thanh toán phí dịch vụ quản lý sân.",
                    IsRead = false,
                    Type = "PayFee"
                };
                _beatSportsDbContext.Notifications.Add(notification);

                var account = _beatSportsDbContext.Accounts.Where(a => a.Id == owner.AccountId).SingleOrDefault();
                if (account != null)
                {
                    if (!string.IsNullOrEmpty(account.Email))
                    {
                        _emailService.SendEmailAsync(
                        account.Email,
                        "Thanh toán phí dịch vụ - BeatSports",
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
                                    Thanh toán phí dịch vụ
                                </div>
                                <div class='content'>
                                    <p>Kính gửi {account.FirstName + " " + account.LastName},</p>
                                    <p>{notification.Message}</p>
                                    <p>Vui lòng đăng nhập vào hệ thống và hoàn tất thanh toán để tránh bất kỳ sự gián đoạn nào trong việc sử dụng dịch vụ.</p>
                                </div>
                                <div class='footer'>
                                    <p>© 2024 BeatSports. All rights reserved.</p>
                                </div>
                            </div>
                        </body>
                        </html>"
                        );
                    }
                }
            }
            _beatSportsDbContext.SaveChanges();
        }
    }
}