using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Campaigns.Queries.GetCampaignById;
public class GetCampaignByIdCommand : IRequest<CampaignResponse>
{
    public Guid CampaignId { get; set; }
}
