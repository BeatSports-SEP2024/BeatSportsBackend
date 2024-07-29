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

namespace BeatSportsAPI.Application.Features.Bookings.Commands.CancelBooking;
public class CancelBookingProcessCommand : IRequest<BeatSportsResponse>
{
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
}

public class CancelBookingProcessCommandHandler : IRequestHandler<CancelBookingProcessCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public CancelBookingProcessCommandHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
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
            _beatSportsDbContext.TimeChecking.Remove(timeChecking);
            _beatSportsDbContext.Bookings.Remove(bookingProcess);
            _beatSportsDbContext.SaveChanges();
        }

        return new BeatSportsResponse
        {
            Message = "Cancel Booking Successfully"
        };
    }
}
