using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMembers.Commands.DeleteRoomMembers;
public class DeleteRoomMemberCommand : IRequest<BeatSportsResponse>
{
    [Required]
    public Guid RoomMemberId { get; set; }
}