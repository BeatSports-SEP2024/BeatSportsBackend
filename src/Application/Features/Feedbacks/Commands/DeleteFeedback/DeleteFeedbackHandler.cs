using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Campaigns.Commands.DeleteCampaign;
using MediatR;

namespace BeatSportsAPI.Application.Features.Feedbacks.Commands.DeleteFeedback;
public class DeleteFeedbackHandler : IRequestHandler<DeleteFeedbackCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public DeleteFeedbackHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<BeatSportsResponse> Handle(DeleteFeedbackCommand request, CancellationToken cancellationToken)
    {
        //check Feedback
        var feedback = _dbContext.Feedbacks.Where(x => x.Id == request.FeedbackId).SingleOrDefault();
        if (feedback == null || feedback.IsDelete)
        {
            throw new BadRequestException($"Feedback with Feedback ID:{request.FeedbackId} does not exist or have been delele");
        }
        feedback.IsDelete = true;
        _dbContext.Feedbacks.Update(feedback);
        _dbContext.SaveChanges();

        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Delete Feedback successfully!"
        });
    }
}
