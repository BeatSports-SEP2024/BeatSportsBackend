using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Campaigns.Queries.GetCampaignByCourtSubdivisionAndTotalMoney;
public class GetCampaignByCourtSubdivisionAndTotalMoneyQueryHandler : IRequestHandler<GetCampaignByCourtSubdivisionAndTotalMoneyQuery, PaginatedList<CampaignResponseV7>>
{
    private readonly IBeatSportsDbContext _dbContext;

    public GetCampaignByCourtSubdivisionAndTotalMoneyQueryHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedList<CampaignResponseV7>> Handle(GetCampaignByCourtSubdivisionAndTotalMoneyQuery request, CancellationToken cancellationToken)
    {
        var courtSub = await _dbContext.CourtSubdivisions.Where(x => x.Id == request.CourtSubdivisionId).SingleAsync();
        var campaign = await _dbContext.Campaigns.Where(x => x.CourtId == courtSub.CourtId && !x.IsDelete && x.Status == Domain.Enums.StatusEnums.Accepted)
            .Select(x => new CampaignResponseV7
            {
                CampaignId = x.Id,
                CampaignName = x.CampaignName,
                EndDateApplying = x.EndDateApplying,
                ExpireCampaign = (x.EndDateApplying - DateTime.Now).Days.ToString(),
                MaxValueDiscount = x.MaxValueDiscount,
                MinValueApply = x.MinValueApply,
                StartDateApplying = x.StartDateApplying,
                Status = x.Status,
                CanApplyCampaign = request.TotalMoney >= x.MinValueApply
            })
            .PaginatedListAsync(request.PageIndex, request.PageSize);
        return campaign;
    }
}
