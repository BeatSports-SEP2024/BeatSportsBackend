using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Courts.Commands.DeleteCourt;
using MediatR;

namespace BeatSportsAPI.Application.Features.Campaigns.Commands.DeleteCampaign;
public class DeleteCampaignHandler : IRequestHandler<DeleteCampaignCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public DeleteCampaignHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<BeatSportsResponse> Handle(DeleteCampaignCommand request, CancellationToken cancellationToken)
    {
        //check Campaign
        var campaign = _dbContext.Campaigns.Where(x => x.Id == request.CampaignId).SingleOrDefault();
        if (campaign == null || campaign.IsDelete)
        {
            throw new BadRequestException($"Campaign with Campaign ID:{request.CampaignId} does not exist or have been delele");
        }
        campaign.IsDelete = true;
        _dbContext.Campaigns.Update(campaign);
        _dbContext.SaveChanges();

        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Delete Campaign successfully!"
        });
    }
}
