using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities.Room;
public class RoomMember
{
    public Guid CustomerId { get; set; }
    public Guid RoomMatchId { get; set; }
    public RoleInRoomEnums RoleInRoom { get; set; }
    /// <summary>
    /// Team A, Team B
    /// </summary>
    public string? Team { get; set; }

    public virtual Customer Customer { get; set; } = null!;
    public virtual RoomMatch RoomMatch { get; set; } = null!;
}
