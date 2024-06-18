using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response.RoomMemberResponse;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMatches.Queries.GetAllMembersById;
public class GetAllMemberByIdCommand : IRequest<PaginatedList<RoomMemberResponse>>
{
    public Guid RoomMatchId { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}