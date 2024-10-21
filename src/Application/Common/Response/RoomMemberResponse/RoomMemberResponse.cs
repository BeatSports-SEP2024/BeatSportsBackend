using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities.Room;
using BeatSportsAPI.Domain.Enums;

namespace BeatSportsAPI.Application.Common.Response.RoomMemberResponse;
public class RoomMemberResponse : IMapFrom<RoomMember>
{
    public Guid CustomerId { get; set; }
    public Guid RoomMatchId { get; set; }
    public RoleInRoomEnums RoleInRoom { get; set; }
}