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
        
        foreach(var booking in bookingList) 
        {
            var endDatePlaying = booking.PlayingDate.Add(booking.EndTimePlaying);
            if(endDatePlaying <= DateTime.Now)
            {
                booking.BookingStatus = BookingEnums.Finished.ToString();
            }
        }
        _beatSportsDbContext.SaveChanges();
    }

    public void CheckBookingPlayDateIfFinish()
    {
        var bookingList = _beatSportsDbContext.Bookings
            .Where(x => !x.IsDelete && x.BookingStatus == BookingEnums.Approved.ToString())
            .ToList();
        
        foreach(var booking in bookingList) 
        {
            var endDatePlaying = booking.PlayingDate.Add(booking.EndTimePlaying);
            if(endDatePlaying <= DateTime.Now)
            {
                booking.BookingStatus = BookingEnums.Finished.ToString();
            }
        }
        _beatSportsDbContext.SaveChanges();
    }

    public void RemoveRoomWhenExpired()
    {
        var expiredRooms = _beatSportsDbContext.RoomMatches
            .Where(x => !x.IsDelete).ToList();

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
}