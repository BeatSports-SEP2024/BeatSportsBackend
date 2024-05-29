using System.ComponentModel.DataAnnotations;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response.RoomMemberResponse;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMembers.Queries.GetAllRoomMember;
public class GetAllRoomMemberCommand : IRequest<PaginatedList<RoomMemberResponse>>
{
    [Required]
    public int PageIndex { get; set; }
    [Required]
    public int PageSize { get; set; }
}