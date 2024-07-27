using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;

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
        var bookingList = _beatSportsDbContext.Bookings.ToList();
        foreach (var booking in bookingList)
        {
            DateTime startTime = booking.PlayingDate.Date.Add(booking.StartTimePlaying);
            DateTime endTime = booking.PlayingDate.Date.Add(booking.EndTimePlaying);

            if (booking.BookingStatus.Equals("Process"))
            {
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
}
