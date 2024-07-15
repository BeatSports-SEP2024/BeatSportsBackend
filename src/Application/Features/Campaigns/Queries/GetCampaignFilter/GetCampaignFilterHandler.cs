using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Campaigns.Queries.GetCampaignFilter;
public class GetCampaignFilterHandler : IRequestHandler<GetCampaignFilterCommand, CampaignResult>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public GetCampaignFilterHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public Task<CampaignResult> Handle(GetCampaignFilterCommand request, CancellationToken cancellationToken)
    {
        var query = _beatSportsDbContext.Campaigns
            .Where(c => !c.IsDelete);

        var pendingCampaigns = query.Where(c => c.Status == 0 && c.Court.Id == request.CourtId)
            .Select(c => new CampaignResponseV4
            {
                CampaignId = c.Id,
                CampaignImageUrl = c.CampaignImageURL,
            }).Take(3).ToList();

        var historyCampaigns = query.Where(c => (int)c.Status == 3)
            .Select(c => new CampaignResponseV4
            {
                CampaignId = c.Id,
                CampaignImageUrl = c.CampaignImageURL,
            }).Take(3).ToList();

        var myCampaigns = _beatSportsDbContext.Campaigns
            .Include(c => c.Court)
            .ThenInclude(court => court.Owner)
            .Where(c => !c.IsDelete && c.Court.Id == request.CourtId && c.Court.Owner.Id == request.OwnerId)
            .Select(c => new CampaignResponseV4
            {
                CampaignId = c.Id,
                CampaignImageUrl = c.CampaignImageURL,
            }).Take(3).ToList();

        var result = new CampaignResult
        {
            PendingCampaigns = pendingCampaigns,
            HistoryCampaigns = historyCampaigns,
            MyCampaigns = myCampaigns
        };

        return Task.FromResult(result);
    }
}