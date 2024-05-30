using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using BeatSportsAPI.Application.Common.Response.RoomMemberResponse;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Domain.Entities.Room;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMembers.Queries.GetAllRoomMemberDetails;
public class GetAllRoomMemberWithDetailHandler : IRequestHandler<GetAllRoomMemberWithDetailCommand, PaginatedList<RoomMemberWithDetailsResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;

    public GetAllRoomMemberWithDetailHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }

    public Task<PaginatedList<RoomMemberWithDetailsResponse>> Handle(GetAllRoomMemberWithDetailCommand request, CancellationToken cancellationToken)
    {
        IQueryable<RoomMember> query = _beatSportsDbContext.RoomMembers
            .AsNoTracking()
            .Include(cus => cus.Customer)
                .ThenInclude(c => c.Account);
        var list = query.Select(s => new RoomMemberWithDetailsResponse
        {
            CustomerId = s.CustomerId,
            RoomMatchId = s.RoomMatchId,
            ProfilePictureURL = s.Customer.Account.ProfilePictureURL,
            FirstName = s.Customer.Account.FirstName,
            LastName = s.Customer.Account.LastName,
        }).PaginatedListAsync(request.PageIndex, request.PageSize);
        return list;
    }
}