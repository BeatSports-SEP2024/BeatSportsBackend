using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMembers.Commands.CreateRoomMembers;
public class CreateRoomMemberCommand : IRequest<BeatSportsResponse>
{
    public Guid CustomerId { get; set; }
    public Guid RoomMatchId { get; set; }
}