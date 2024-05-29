using System.ComponentModel.DataAnnotations.Schema;
using BeatSportsAPI.Domain.Entities.CourtEntity;

namespace BeatSportsAPI.Domain.Entities;
public class Feedback : BaseAuditableEntity
{
    [ForeignKey("Booking")]
    public Guid BookingId { get; set; }
    [ForeignKey("Court")]
    public Guid CourtId { get; set; }
    public decimal FeedbackStar {  get; set; }
    public bool FeedbackAvailable { get; set; }
    public string? FeedbackStatus { get; set; }
    public string FeedbackContent { get; set; } = null!;

    public virtual Booking Booking { get; set; } = null!;
    public virtual Court Court { get; set; } = null!;
}
