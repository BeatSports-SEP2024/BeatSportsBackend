using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomRequests.Commands.CreateRoomRequests;
public class CreateRoomRequestCommand : IRequest<BeatSportsResponse>
{
    public Guid CustomerId { get; set; }
    public Guid RoomMatchId { get; set; }
}