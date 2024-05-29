using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response.RoomMemberResponse;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMembers.Queries.GetAllRoomMemberDetails;
public class GetAllRoomMemberWithDetailCommand : IRequest<PaginatedList<RoomMemberWithDetailsResponse>>
{
    [Required]
    public int PageIndex { get; set; }
    [Required]
    public int PageSize { get; set; }
}
