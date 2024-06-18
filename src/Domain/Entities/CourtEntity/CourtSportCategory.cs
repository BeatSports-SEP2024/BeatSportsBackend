using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities.CourtEntity;
public class CourtSportCategory
{
    public Guid CourtSubdivisionId { get; set; }
    public Guid SportCategoryId { get; set; }

    public virtual CourtSubdivision CourtSubdivision { get; set; }
    public virtual SportCategory SportCategory { get; set; }
}
