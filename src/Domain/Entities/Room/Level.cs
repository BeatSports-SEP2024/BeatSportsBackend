using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities.Room;
public class Level : BaseAuditableEntity
{
    [ForeignKey("RoomMatch")]
    public Guid RoomMatchId { get; set; }
    public string? LevelName { get; set; }
    public string? LevelDescription { get; set; }

    public virtual RoomMatch RoomMatch { get; set; }
}
