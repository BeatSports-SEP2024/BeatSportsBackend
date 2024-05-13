using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities;
public class Admin : BaseAuditableEntity
{
    [ForeignKey("Account")]
    public Guid AccountId { get; set; }

    public virtual Account Account { get; set; }
}
