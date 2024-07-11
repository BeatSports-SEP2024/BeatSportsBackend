using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Owners.Queries.GetAllOwners;
public class GetAllOwnersHandler : IRequestHandler<GetAllOwnersCommand, PaginatedList<OwnerResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;
    public GetAllOwnersHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }
    public async Task<PaginatedList<OwnerResponse>> Handle(GetAllOwnersCommand request, CancellationToken cancellationToken)
    {
        if (request.PageIndex <= 0 || request.PageSize <= 0)
        {
            throw new BadRequestException("Page index and page size cannot less than 0");
        }

        IQueryable<Owner> query = _beatSportsDbContext.Owners
            .Where(x => !x.IsDelete)
            .OrderByDescending(b => b.Created);

        var list = query.Select(c => new OwnerResponse
        {
            AccountId = c.AccountId,
            OwnerId = c.Id,
            UserName = c.Account.UserName,
            Email = c.Account.Email,
            FirstName = c.Account.FirstName,
            LastName = c.Account.LastName,
            DateOfBirth = c.Account.DateOfBirth,
            Gender = c.Account.Gender,
            ProfilePictureURL = c.Account.ProfilePictureURL,
            Bio = c.Account.Bio,
            PhoneNumber = c.Account.PhoneNumber,
            Address = c.Address
        })
        .PaginatedListAsync(request.PageIndex, request.PageSize);

        return await list;
    }
}
