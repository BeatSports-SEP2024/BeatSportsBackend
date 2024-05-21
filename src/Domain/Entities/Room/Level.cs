using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities.Room;
public class Level : BaseAuditableEntity
{
    public string? LevelName { get; set; }
    public string? LevelDescription { get; set; }

    public virtual IList<RoomMatch> RoomMatch { get; set; }
}
