using System.ComponentModel.DataAnnotations.Schema;
using BeatSportsAPI.Domain.Entities.CourtEntity;

namespace BeatSportsAPI.Domain.Entities;
public class Owner : BaseAuditableEntity
{
    [ForeignKey("Account")]
    public Guid AccountId { get; set; }
    public string? Address { get; set; } 

    public virtual Account Account { get; set; } = null!;
    public virtual IList<Court>? ListCourt { get; set; }
}
