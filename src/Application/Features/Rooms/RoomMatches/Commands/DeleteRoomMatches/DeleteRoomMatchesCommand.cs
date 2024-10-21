using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMatches.Commands.DeleteRoomMatches;
public class DeleteRoomMatchesCommand : IRequest<BeatSportsResponse>
{
    public Guid RoomMatchId { get; set; }
}
