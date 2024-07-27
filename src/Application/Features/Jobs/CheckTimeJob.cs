using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

                if (timeChecking != null)
                {
                    _beatSportsDbContext.TimeChecking.Remove(timeChecking);
                    _beatSportsDbContext.Bookings.Remove(booking);
                    _beatSportsDbContext.SaveChanges();

                    Console.WriteLine($"Recurring job executed start for Booking ${booking.Id}");
                }
            }
        }
    }
}
