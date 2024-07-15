using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Campaigns.Queries.GetCampaignFilter;
public class GetCampaignFilterHandler : IRequestHandler<GetCampaignFilterCommand, List<CampaignResponseV4>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public GetCampaignFilterHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public Task<List<CampaignResponseV4>> Handle(GetCampaignFilterCommand request, CancellationToken cancellationToken)
    {
        var query = _beatSportsDbContext.Campaigns
            .Where(c => !c.IsDelete);

        switch (request.CampaignFilter.ToString()) 
        {
            case "Pending":
                query = query.Where(tp => tp.Status == 0);
            break;

            case "History":
                query = query.Where(tp => tp.EndDateApplying <= DateTime.UtcNow);
            break;

            default:
                throw new BadRequestException("Invalid filter");
        }

        var list = query.Select(q => new CampaignResponseV4
        {
            CampaignId = q.Id,
            CampaignImageUrl = q.CampaignImageURL
        }).Take(3).ToList();

        return Task.FromResult(list);
    }
}