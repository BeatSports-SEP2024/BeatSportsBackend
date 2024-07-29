using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Campaigns.Commands.CreateCampaign;
using BeatSportsAPI.Domain.Entities;
using MediatR;

namespace BeatSportsAPI.Application.Features.Feedbacks.Commands.CreateFeedback;
public class CreateFeedbackHandler : IRequestHandler<CreateFeedbackCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public CreateFeedbackHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<BeatSportsResponse> Handle(CreateFeedbackCommand request, CancellationToken cancellationToken)
    {
        //check Booking
        var booking = _dbContext.Bookings.Where(x => x.Id == request.BookingId).SingleOrDefault();
        if (booking == null || booking.IsDelete)
        {
            throw new NotFoundException($"Booking with Booking ID:{request.BookingId} does not exist or have been delele");
        }

        //check Court
        var subCourt = _dbContext.CourtSubdivisions.Where(c => c.Id == booking.CourtSubdivisionId).SingleOrDefault();
        if (subCourt == null || subCourt.IsDelete)
        {
            throw new NotFoundException($"Sub Court Not found");
        }
        var court = _dbContext.Courts.Where(x => x.Id == subCourt.CourtId).SingleOrDefault();
        if (court == null || court.IsDelete)
        {
            throw new NotFoundException($"Court Not found");
        }
        var feedback = new Feedback()
        {
            BookingId = request.BookingId,
            CourtId = court.Id,
            FeedbackStar = request.FeedbackStar,
            FeedbackAvailable = true,
            //FeedbackStatus = request.FeedbackStatus,
            FeedbackContent = request.FeedbackContent,
        };
        _dbContext.Feedbacks.Add(feedback);
        _dbContext.SaveChanges();
        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Create Feedback successfully!"
        });
    }
}
