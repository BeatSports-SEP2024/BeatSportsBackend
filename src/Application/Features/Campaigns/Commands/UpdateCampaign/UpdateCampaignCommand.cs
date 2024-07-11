using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Campaigns.Commands.UpdateCampaign;
public class UpdateCampaignCommand : IRequest<BeatSportsResponse>
{
    
    [Required]
    public Guid CampaignId { get; set; }
    public string? CampaignName { get; set; }
    public string? Description { get; set; }
    public decimal PercentDiscount { get; set; }
    public DateTime StartDateApplying { get; set; }
    public DateTime EndDateApplying { get; set; }
    [EnumDataType(typeof(SportCategoriesEnums))]
    public SportCategoriesEnums SportTypeApply { get; set; }
    public decimal MinValueApply { get; set; }
    public decimal MaxValueDiscount { get; set; }
    public StatusEnums Status { get; set; }
    public int QuantityOfCampaign { get; set; }
    public string CampaignImageUrl { get; set; }
    public string ReasonOfReject { get; set; }
}
