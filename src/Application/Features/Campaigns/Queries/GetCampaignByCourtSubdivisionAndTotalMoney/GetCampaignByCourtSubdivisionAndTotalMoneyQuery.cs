using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Campaigns.Queries.GetCampaignByCourtSubdivisionAndTotalMoney;
public class GetCampaignByCourtSubdivisionAndTotalMoneyQuery : IRequest<PaginatedList<CampaignResponseV7>>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public Guid CourtSubdivisionId { get;set; }
    public decimal TotalMoney { get; set; } 
}
