using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Campaigns.Commands.DeleteCampaign;
public class DeleteCampaignCommand : IRequest<BeatSportsResponse>
{
    public Guid CampaignId { get; set; }
}
