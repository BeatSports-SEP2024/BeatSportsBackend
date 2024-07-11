using System.ComponentModel.DataAnnotations.Schema;
using BeatSportsAPI.Domain.Entities.CourtEntity;

namespace BeatSportsAPI.Domain.Entities;
public class Campaign : BaseAuditableEntity
{
    [ForeignKey("Court")]
    public Guid CourtId { get; set; }   
    public string? CampaignName { get; set; }
    public string? Description { get; set; }
    public decimal PercentDiscount { get; set; }
    //Ap dung campaign cho loai hinh the thao nao
    public string? SportTypeApply { get; set; }
    //Gia tri toi thieu de ap dung campaign
    public decimal MinValueApply { get; set; }
    //Gia tri toi da duoc giam gia
    public decimal MaxValueDiscount { get; set; }
    public DateTime StartDateApplying { get; set; }
    public DateTime EndDateApplying { get;set; }
    public StatusEnums Status { get; set; }
    public int QuantityOfCampaign { get; set; }
    public string? CampaignImageURL { get; set; }
    public string? ReasonOfReject { get; set; }
    public virtual Court Court { get; set; } = null!;
    public virtual IList<Booking> Booking { get; set; } = null!;
}
