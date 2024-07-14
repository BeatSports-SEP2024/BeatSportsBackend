using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Campaigns.Queries.GetCampaignListByFilter;
public class GetCampaignListByFilterHandler : IRequestHandler<GetCampaignListByFilterCommand, PaginatedList<CampaignResponseV5>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public GetCampaignListByFilterHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public Task<PaginatedList<CampaignResponseV5>> Handle(GetCampaignListByFilterCommand request, CancellationToken cancellationToken)
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

        var list = query.Select(q => new CampaignResponseV5
        {
            CampaignId = q.Id,
            StartDateApplying = q.StartDateApplying,
            EndDateApplying = q.EndDateApplying,
            Status = q.Status,
            MinValueApply = q.MinValueApply,
            MaxValueDiscount = q.MaxValueDiscount,            
        }).PaginatedListAsync(request.PageIndex, request.PageSize);

        return list;
    }
}