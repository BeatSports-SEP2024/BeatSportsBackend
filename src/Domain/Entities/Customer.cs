using System.ComponentModel.DataAnnotations.Schema;
using BeatSportsAPI.Domain.Entities.CourtEntity;

namespace BeatSportsAPI.Domain.Entities;
public class Customer : BaseAuditableEntity
{
    [ForeignKey("Account")]
    public Guid AccountId { get; set; }
    public int RewardPoints { get; set; }
    public string Address { get; set; }

    public virtual Account Account { get; set; }
    public virtual IList<BookingDetail> BookingDetails { get; set; }
}
