using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMatches.Queries.GetAllRoomMatches;
public class GetAllRoomMatchesCommand : IRequest<RoomRequestsResponseForGetAll>
{
    //[Required]
    //public int PageIndex { get; set; }
    //[Required]
    //public int PageSize { get; set; }
    public Guid CustomerId { get; set; }
}