using BeatSportsAPI.Domain.Entities.CourtEntity;

namespace BeatSportsAPI.Domain.Entities;
public class SportCategory : BaseAuditableEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImageURL { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual IList<CourtSportCategory> CourtSportCategories { get; set; }
}
