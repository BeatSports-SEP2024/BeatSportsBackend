using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomRequests.Queries.GetRoomRequestByCustomer;
public class GetRoomRequestByCustomerCommand : IRequest<List<RoomRequestResponseForCustomer>>
{
    public Guid CustomerId { get; set; }
}