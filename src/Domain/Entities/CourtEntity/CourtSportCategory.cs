using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities.CourtEntity;
public class CourtSportCategory
{
    public Guid CourtId { get; set; }
    public Guid SportCategoryId { get; set; }

    public virtual Court Court { get; set; }
    public virtual SportCategory SportCategory { get; set; }
}
