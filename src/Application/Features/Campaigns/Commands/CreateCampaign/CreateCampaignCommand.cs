using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Campaigns.Commands.CreateCampaign;
public class CreateCampaignCommand : IRequest<BeatSportsResponse>
{
    [Required]
    public Guid CourtId { get; set; }
    public string? CampaignName { get; set; }
    public string? Description { get; set; }
    public decimal PercentDiscount { get; set; }
    public DateTime StartDateApplying { get; set; }
    public DateTime EndDateApplying { get; set; }
    public int QuantityOfCampaign { get; set; }
    public string CampaignImageUrl { get; set; }
}
