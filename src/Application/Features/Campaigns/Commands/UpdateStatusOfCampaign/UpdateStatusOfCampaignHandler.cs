using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Campaigns.Commands.UpdateCampaign;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Campaigns.Commands.UpdateStatusOfCampaign;
public class UpdateStatusOfCampaignHandler : IRequestHandler<UpdateStatusOfCampaignCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public UpdateStatusOfCampaignHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<BeatSportsResponse> Handle(UpdateStatusOfCampaignCommand request, CancellationToken cancellationToken)
    {
        // Check Campaign
        var campaign = _dbContext.Campaigns.Where(x => x.Id == request.CampaignId).SingleOrDefault();
        if (campaign == null || campaign.IsDelete)
        {
            throw new BadRequestException($"Campaign with Campaign ID:{request.CampaignId} does not exist or have been delele");
        }

        campaign.Status = StatusEnums.Accepted;
       
        _dbContext.Campaigns.Update(campaign);
        _dbContext.SaveChanges();

        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Update Campaign successfully!"
        });
    }
}
