using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using BeatSportsAPI.Domain.Enums;

namespace BeatSportsAPI.Application.Common.Response;
public class CampaignResponse : IMapFrom<Campaign>
{
    public Guid Id { get; set; }
    public Guid CourtId { get; set; }
    public string OwnerName { get; set; }
    public string? CampaignName { get; set; }
    public string? Description { get; set; }
    public decimal PercentDiscount { get; set; }
    public DateTime StartDateApplying { get; set; }
    public DateTime EndDateApplying { get; set; }
    public string SportTypeApply { get; set; }
    public decimal MinValueApply { get; set; }
    public decimal MaxValueDiscount { get; set; }
    public StatusEnums Status { get; set; }
    public int QuantityOfCampaign { get; set; }
    public string CampaignImageUrl { get; set; }
    public string ReasonOfReject { get; set; }
    public DateTime Created { get; set; }
    public Court Court { get; set; } = null!;
}

public class CampaignResponseV2 
{
    public Guid CampaignId { get; set; }
    public Guid CourtId { get; set; }
    public string? CampaignName { get; set; }
    public DateTime StartDateApplying { get; set; }
    public DateTime EndDateApplying { get; set; }
    public string SportTypeApply { get; set; }
    public DateTime Created { get; set; }
    public StatusEnums Status { get; set; }
}

public class CampaignResponseV3 
{
    public Guid Id { get; set; }
    public Guid CourtId { get; set; }
    public string OwnerName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AddressOfOwner { get; set; }
    public string CourtName { get; set; }
    public string? AddressOfCourt { get; set; }
    public string? DescriptionOfCourt { get; set; }
    public TimeSpan TimeStartOfCourt { get; set; }
    public TimeSpan TimeEndOfCourt { get; set; }
    public int NumberOfCourtSub { get; set; }
    public string? CampaignName { get; set; }
    public string? DescriptionOfCampaign { get; set; }
    public decimal PercentDiscount { get; set; }
    public DateTime StartDateApplying { get; set; }
    public DateTime EndDateApplying { get; set; }
    public string SportTypeApply { get; set; }
    public decimal MinValueApply { get; set; }
    public decimal MaxValueDiscount { get; set; }
    public StatusEnums Status { get; set; }
    public int QuantityOfCampaign { get; set; }
    public string CampaignImageUrl { get; set; }
    public string ReasonOfReject { get; set; }
    public DateTime Created { get; set; }
}

public class CampaignResponseV4
{
    public Guid CampaignId { get; set; }
    public string CampaignImageUrl { get; set; }
}

public class CampaignResponseV5
{
    public Guid CampaignId { get; set; }
    public StatusEnums Status { get; set; }
    public string? CampaignName { get; set; }
    public DateTime StartDateApplying { get; set; }
    public DateTime EndDateApplying { get; set; }
    public string? ExpireCampaign { get; set; }
    public decimal MinValueApply { get; set; }
    public decimal MaxValueDiscount { get; set; }
}

public class CampaignResult
{
    public List<CampaignResponseV4> PendingCampaigns { get; set; }
    public List<CampaignResponseV4> HistoryCampaigns { get; set; }
    public List<CampaignResponseV4> MyCampaigns { get; set; }
}