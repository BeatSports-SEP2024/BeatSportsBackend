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
    /// <summary>
    /// 1. Tạo phòng chưa có kết quả (NoResult)
    /// 2. Thắng (Win)
    /// 3. Thua (Lose)
    /// </summary>
    public string? MatchingResultStatus {  get; set; } 

    public virtual Customer Customer { get; set; } = null!;
    public virtual RoomMatch RoomMatch { get; set; } = null!;
}
