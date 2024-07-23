using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMatches.Queries.GetRoomMatchById;
public class GetRoomMatchByIdCommand : IRequest<RoomMatchesDetailResponse>
{
    public Guid RoomMatchId { get; set; }
    public Guid CustomerId { get; set; }
}
