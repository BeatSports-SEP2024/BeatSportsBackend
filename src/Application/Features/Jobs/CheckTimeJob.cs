using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Domain.Enums;

namespace BeatSportsAPI.Application.Features.Jobs;
public class CheckTimeJob
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public CheckTimeJob(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }
    public void CheckTimeOfCourt()
    {
       var bookingList = _beatSportsDbContext.Bookings.Where(x => x.BookingStatus == BookingEnums.Process.ToString()).ToList();
        foreach (var booking in bookingList)
        {

            var checkAfter2Minutes = booking.Created.AddMinutes(2);
            if(DateTime.Now > checkAfter2Minutes)
            {
                DateTime startTime = booking.PlayingDate.Date.Add(booking.StartTimePlaying);
                DateTime endTime = booking.PlayingDate.Date.Add(booking.EndTimePlaying);
                var timeChecking = _beatSportsDbContext.TimeChecking
                                   .Where(x => x.CourtSubdivisionId == booking.CourtSubdivisionId
                                   && x.StartTime == startTime && x.EndTime == endTime && x.DateBooking == booking.PlayingDate)
                                   .FirstOrDefault();


                if(timeChecking != null)
                {
                    timeChecking.IsDelete = true;
                    _beatSportsDbContext.TimeChecking.Update(timeChecking);
                    _beatSportsDbContext.SaveChanges();

                    booking.IsDelete = true;
                    _beatSportsDbContext.Bookings.Update(booking);
                    _beatSportsDbContext.SaveChanges();

                    Console.WriteLine($"Recurring job executed start for Booking ${booking.Id}");
                }
            }
        }
    }

    public void CheckTimeOfBooking()
    {
        var bookingList = _beatSportsDbContext.Bookings
                        .Where(x => !x.IsDelete && x.BookingStatus.Equals("Pending"))
                        .ToList();

        foreach (var booking in bookingList)
        {
            // Chuyển đổi Unix timestamp ngược lại thành DateTime
            // DateTimeOffset dateTimeOffsetFromUnix = DateTimeOffset.FromUnixTimeSeconds(bookingApprove.UnixTimestampMinCancellation);
            DateTime datetimeFromUnix = booking.UnixTimestampMinCancellation;
            // Ghép PlayingDate và StartTimePlaying lại
            DateTime playingStartDateTime = booking.PlayingDate.Date.Add(booking.StartTimePlaying - datetimeFromUnix.TimeOfDay);

            // Lấy thời gian hiện tại
            DateTime currentDateTime = DateTime.Now;

            // So sánh thời gian hủy với thời gian hiện tại
            TimeSpan timeDifference = playingStartDateTime - currentDateTime;

            if (timeDifference <= TimeSpan.Zero)
            {
                // Thời gian hủy nhỏ hơn hoặc bằng thời gian hiện tại
                //throw new BadRequestException($"Không thể hủy đặt sân, thời gian tối thiểu để hủy lịch đặt sân đã trôi qua. Thời gian bị lệch: {timeDifference}");
                var transaction = _beatSportsDbContext.Transactions
                                .Where(x => x.Id == booking.TransactionId && x.AdminCheckStatus == AdminCheckEnums.Pending)
                                .FirstOrDefault();
                if(transaction != null)
                {
                    var ownerWallet = _beatSportsDbContext.Wallets
                                .Where(x => x.Id == transaction.WalletTargetId)
                                .FirstOrDefault();

                    transaction.AdminCheckStatus = AdminCheckEnums.Accepted;

                    ownerWallet.Balance += (int)transaction.TransactionAmount;

                    _beatSportsDbContext.Transactions.Update(transaction);
                    _beatSportsDbContext.Wallets.Update(ownerWallet);
                    _beatSportsDbContext.SaveChanges();
                }
                
            }
        }
        
    }
}
