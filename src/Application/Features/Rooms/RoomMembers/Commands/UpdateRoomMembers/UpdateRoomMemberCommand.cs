using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMembers.Commands.UpdateRoomMembers;
public class UpdateRoomMemberCommand : IRequest<BeatSportsResponse>
{
    [Required]
    public Guid RoomMemberId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid RoomMatchId { get; set; }
    [EnumDataType(typeof(RoleInRoomEnums))]
    public RoleInRoomEnums RoleInRoom { get; set; }
}