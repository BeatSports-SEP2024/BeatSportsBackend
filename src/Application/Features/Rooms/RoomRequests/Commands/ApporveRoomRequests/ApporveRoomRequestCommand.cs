using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomRequests.Commands.UpdateRoomRequests;
public class ApporveRoomRequestCommand : IRequest<BeatSportsResponse>
{
    public Guid RoomRequestId { get; set; }
    public RoomRequestEnums RoomRequest{ get; set; }
}