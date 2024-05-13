using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities;
public class Customer : BaseAuditableEntity
{
    [ForeignKey("Account")]
    public Guid AccountId { get; set; }

    public virtual Account Account { get; set; }
}
