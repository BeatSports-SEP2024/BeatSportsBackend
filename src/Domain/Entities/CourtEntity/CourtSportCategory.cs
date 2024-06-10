using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities.CourtEntity;
public class CourtSportCategory : BaseAuditableEntity
{
    [ForeignKey("Court")]
    public Guid CourtId { get; set; }
    [ForeignKey("SportCategory")]
    public Guid SportCategoryId { get; set; }

    public virtual Court Court { get; set; }
    public virtual SportCategory SportCategory { get; set; }
}
