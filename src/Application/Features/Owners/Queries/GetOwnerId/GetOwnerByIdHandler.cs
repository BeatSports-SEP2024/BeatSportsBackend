using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Exceptions;
using MediatR;
using BeatSportsAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using BeatSportsAPI.Application.Features.Owners.Queries.GetOwnerId;

namespace BeatSportsAPI.Application.Features.Owners.Queries;
public class GetOwnerByIdHandler : IRequestHandler<GetOwnerByIdCommand, OwnerResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;

    public GetOwnerByIdHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }

    public Task<OwnerResponse> Handle(GetOwnerByIdCommand request, CancellationToken cancellationToken)
    {
        IQueryable<Owner> query = _beatSportsDbContext.Owners
            .Where(x => x.Id == request.OwnerId && !x.IsDelete)
            .Include(c => c.Account);

        var owner = query.Select(c => new OwnerResponse
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
            Address = c.Address,
        }).SingleOrDefault();

        if (owner == null)
        {
            throw new NotFoundException($"Do not find owner with owner ID: {request.OwnerId}");
        }
        return Task.FromResult(owner);
    }
}
