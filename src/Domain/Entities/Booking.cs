using System.ComponentModel.DataAnnotations.Schema;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using BeatSportsAPI.Domain.Entities.PaymentEntity;
using BeatSportsAPI.Domain.Entities.Room;

namespace BeatSportsAPI.Domain.Entities;
public class Booking : BaseAuditableEntity
{
    [ForeignKey("Customer")]
    public Guid CustomerId { get; set; }
    //[ForeignKey("RoomMatch")]
    //public Guid? RoomMatchId { get; set; }
    [ForeignKey("Campaign")]
    public Guid? CampaignId { get; set; }
    [ForeignKey("CourtSubdivision")]
    public Guid CourtSubdivisionId { get; set; }
    public DateTime BookingDate { get; set; }   
    public decimal TotalAmount { get; set; }
    public bool IsRoomBooking { get; set; } 
    public bool IsDeposit {  get; set; }
    public DateTime PlayingDate { get; set; }
    public TimeSpan StartTimePlaying { get; set; }
    public TimeSpan EndTimePlaying { get; set; }
    public string BookingStatus { get; set; }
    public virtual CourtSubdivision CourtSubdivision { get; set; } = null!;
    public virtual Customer Customer { get; set; } = null!;
    public virtual Campaign? Campaign { get; set; }
    public virtual Feedback? Feedback { get; set; }
    public virtual RoomMatch? RoomMatch { get; set; }
}
