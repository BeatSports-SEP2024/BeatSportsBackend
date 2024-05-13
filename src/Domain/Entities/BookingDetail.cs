using System.ComponentModel.DataAnnotations.Schema;
using BeatSportsAPI.Domain.Entities.CourtEntity;

namespace BeatSportsAPI.Domain.Entities;
public class BookingDetail : BaseAuditableEntity
{
    [ForeignKey("Booking")]
    public Guid BookingId { get; set; }
    [ForeignKey("Court")]
    public Guid CourtId { get; set; }   

    public virtual Booking Booking { get; set; }
    public virtual Court Court { get; set; }
}
