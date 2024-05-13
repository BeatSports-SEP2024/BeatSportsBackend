using System.ComponentModel.DataAnnotations.Schema;
using BeatSportsAPI.Domain.Entities.PaymentEntity;

namespace BeatSportsAPI.Domain.Entities;
public class Booking : BaseAuditableEntity
{
    [ForeignKey("Customer")]
    public Guid CustomerId { get; set; }
    [ForeignKey("Payment")]
    public Guid PaymentId { get; set; }
    public DateTime BookingDate { get; set; }   
    public DateTime PlayingDate { get; set; }   
    public decimal TotalAmount { get; set; }

    public virtual Customer Customer { get; set; }
    public virtual Payment Payment { get; set; }
    public virtual IList<BookingDetail> BookingDetails { get; set; }
}
