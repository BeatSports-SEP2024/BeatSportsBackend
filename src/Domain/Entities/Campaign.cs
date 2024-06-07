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
    public DateTime StartDateApplying { get; set; }
    public DateTime EndDateApplying { get;set; }
    public bool Status { get; set; }
    public int QuantityOfCampaign { get; set; } 

    public virtual Court Court { get; set; } = null!;
    public virtual IList<Booking> Booking { get; set; } = null!;
}
