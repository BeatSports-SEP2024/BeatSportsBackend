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
            throw new BadRequestException($"Booking with Booking ID:{request.BookingId} does not exist or have been delele");
        }

        //check Court
        var court = _dbContext.Courts.Where(x => x.Id == request.CourtId).SingleOrDefault();
        if (court == null || court.IsDelete)
        {
            throw new BadRequestException($"Court with Court ID:{request.CourtId} does not exist or have been delele");
        }
        var feedback = new Feedback()
        {
            BookingId = request.BookingId,
            CourtId = request.CourtId,
            FeedbackStar = request.FeedbackStar,
            FeedbackAvailable = request.FeedbackAvailable,
            FeedbackStatus = request.FeedbackStatus,
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
