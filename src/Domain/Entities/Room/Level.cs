namespace BeatSportsAPI.Domain.Entities.Room;
public class Level : BaseAuditableEntity
{
    public string? LevelName { get; set; }
    public string? LevelDescription { get; set; }

    public virtual RoomMatch RoomMatch { get; set; }
}
