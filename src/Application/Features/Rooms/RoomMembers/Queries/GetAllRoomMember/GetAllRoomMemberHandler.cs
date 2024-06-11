using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response.RoomMemberResponse;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMembers.Queries.GetAllRoomMember;
public class GetAllRoomMemberHandler : IRequestHandler<GetAllRoomMemberCommand, PaginatedList<RoomMemberResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;

    public GetAllRoomMemberHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }

    public Task<PaginatedList<RoomMemberResponse>> Handle(GetAllRoomMemberCommand request, CancellationToken cancellationToken)
    {
        var roomMember = _beatSportsDbContext.RoomMembers
            .ProjectTo<RoomMemberResponse>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageIndex, request.PageSize);
        return roomMember;
    }
}
