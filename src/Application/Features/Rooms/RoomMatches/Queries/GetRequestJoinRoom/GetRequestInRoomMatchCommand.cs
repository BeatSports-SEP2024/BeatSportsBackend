using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMatches.Queries.GetRequestJoinRoom;
public class GetRequestInRoomMatchCommand : IRequest<GetRoomRequestInRoom>
{
    public Guid? RoomMatchId { get; set; }
}