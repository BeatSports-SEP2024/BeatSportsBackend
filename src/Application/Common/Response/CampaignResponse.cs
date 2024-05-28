using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.CourtEntity;

namespace BeatSportsAPI.Application.Common.Response;
public class CampaignResponse : IMapFrom<Campaign>
{
    public Guid CampaignId { get; set; }
    public Guid CourtId { get; set; }
    public string? CampaignName { get; set; }
    public string? Description { get; set; }
    public decimal PercentDiscount { get; set; }
    public DateTime StartDateApplying { get; set; }
    public DateTime EndDateApplying { get; set; }
    public bool Status { get; set; }
    public int QuantityOfCampaign { get; set; }
}
