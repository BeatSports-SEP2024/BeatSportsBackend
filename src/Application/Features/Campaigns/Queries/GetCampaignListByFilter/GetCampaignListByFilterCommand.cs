using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Campaigns.Queries.GetCampaignListByFilter;
public class GetCampaignListByFilterCommand : IRequest<PaginatedList<CampaignResponseV5>>
{
    [Required]
    public int PageIndex { get; set; }
    [Required]
    public int PageSize { get; set; }
    [EnumDataType(typeof(CampaignFilterEnum))]
    public CampaignFilterEnum CampaignFilter { get; set; }
}