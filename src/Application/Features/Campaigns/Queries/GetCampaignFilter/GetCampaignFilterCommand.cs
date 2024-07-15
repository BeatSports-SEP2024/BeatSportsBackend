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

namespace BeatSportsAPI.Application.Features.Campaigns.Queries.GetCampaignFilter;
public class GetCampaignFilterCommand : IRequest<List<CampaignResponseV4>>
{
    [EnumDataType(typeof(CampaignFilterEnum))]
    public CampaignFilterEnum CampaignFilter { get; set; }
}