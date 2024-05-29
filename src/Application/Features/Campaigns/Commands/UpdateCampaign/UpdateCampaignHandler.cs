using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Courts.Commands.UpdateCourt;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;

namespace BeatSportsAPI.Application.Features.Campaigns.Commands.UpdateCampaign;
public class UpdateCampaignHandler : IRequestHandler<UpdateCampaignCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public UpdateCampaignHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<BeatSportsResponse> Handle(UpdateCampaignCommand request, CancellationToken cancellationToken)
    {
        // Check Campaign
        var campaign = _dbContext.Campaigns.Where(x => x.Id == request.CampaignId).SingleOrDefault();
        if (campaign == null || campaign.IsDelete)
        {
            throw new BadRequestException($"Campaign with Campaign ID:{request.CampaignId} does not exist or have been delele");
        }

        campaign.CampaignName = request.CampaignName;
        campaign.Description = request.Description;
        campaign.PercentDiscount = request.PercentDiscount;
        campaign.StartDateApplying = request.StartDateApplying;
        campaign.EndDateApplying = request.EndDateApplying;
        campaign.Status = request.Status;
        campaign.QuantityOfCampaign = request.QuantityOfCampaign;

        _dbContext.Campaigns.Update(campaign);
        _dbContext.SaveChanges();

        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Update Campaign successfully!"
        });
    }
}
