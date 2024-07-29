﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;
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

    public CancelBookingApproveCommandHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<BeatSportsResponse> Handle(CancelBookingApproveCommand request, CancellationToken cancellationToken)
    {
        var bookingApprove = await _beatSportsDbContext.Bookings
            .Where(x => x.BookingStatus == BookingEnums.Approved.ToString()
            && x.Id == request.BookingId
            && x.CustomerId == request.CustomerId).FirstOrDefaultAsync();
        if (bookingApprove == null)
        {
            throw new NotFoundException("Not Found");
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
            _beatSportsDbContext.SaveChanges();
        }

        // check campaign
        if (bookingApprove.CampaignId != null)
        {
            var campaignExist = await _beatSportsDbContext.Campaigns.Where(c => c.Id == bookingApprove.CampaignId).FirstOrDefaultAsync();
            campaignExist!.QuantityOfCampaign += 1;
            _beatSportsDbContext.Campaigns.Update(campaignExist);
        }

        bookingApprove.BookingStatus = BookingEnums.Cancel.ToString();
        _beatSportsDbContext.Bookings.Update(bookingApprove);
        await _beatSportsDbContext.SaveChangesAsync();

        return new BeatSportsResponse
        {
            Message = "Cancel Booking Successfully"
        };
    }
}
