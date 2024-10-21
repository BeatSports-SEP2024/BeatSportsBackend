using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Campaigns.Commands.UpdateCampaign;
using BeatSportsAPI.Domain.Entities;
using MediatR;

namespace BeatSportsAPI.Application.Features.Feedbacks.Commands.UpdateFeedback;
public class UpdateFeedbackHandler : IRequestHandler<UpdateFeedbackCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public UpdateFeedbackHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<BeatSportsResponse> Handle(UpdateFeedbackCommand request, CancellationToken cancellationToken)
    {
        // Check Feedback
        var feedback = _dbContext.Feedbacks.Where(x => x.Id == request.FeedbackId).SingleOrDefault();
        if (feedback == null || feedback.IsDelete)
        {
            throw new BadRequestException($"Feedback with Feedback ID:{request.FeedbackId} does not exist or have been delele");
        }

        feedback.FeedbackStar = request.FeedbackStar;
        feedback.FeedbackAvailable = request.FeedbackAvailable;
        feedback.FeedbackStatus = request.FeedbackStatus;
        feedback.FeedbackContent = request.FeedbackContent;

        _dbContext.Feedbacks.Update(feedback);
        _dbContext.SaveChanges();

        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Update Feedback successfully!"
        });
    }
}
