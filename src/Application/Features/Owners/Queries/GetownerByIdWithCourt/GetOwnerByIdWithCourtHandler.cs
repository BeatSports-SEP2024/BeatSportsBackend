using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Owners.Queries.GetownerByIdWithCourt;
public class GetOwnerByIdWithCourtHandler : IRequestHandler<GetOwnerByIdWithCourtCommand, OwnerResponseV2>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public GetOwnerByIdWithCourtHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public Task<OwnerResponseV2> Handle(GetOwnerByIdWithCourtCommand request, CancellationToken cancellationToken)
    {
        var query = _beatSportsDbContext.Owners
            .Where(o => o.Id == request.OwnerId && !o.IsDelete)
            .OrderByDescending(c => c.Created)
            .Include(o => o.Account).ThenInclude(o => o.Wallet)
            .Include(o => o.ListCourt)
                .ThenInclude(o => o.CourtSubdivision)
            .FirstOrDefault();

        if(query == null)
        {
            throw new NotFoundException("Owner is not existed");
        }

        var listCourt = query.ListCourt.Select(c => new CourtResponseV4
        {
            CourtId = c.Id,
            Address = c.Address,
            CourtName = c.CourtName,
            Description = c.Description,
            CourtSubdivision = c.CourtSubdivision.Select(subCourt => new CourtSubdivisionV2 
            {
                CourtSubdivisionId = subCourt.Id,
                CourtSubdivisionName = subCourt.CourtSubdivisionName,
                Description = subCourt.CourtSubdivisionDescription,
                BasePrice = subCourt.BasePrice,
                StartTime = c.TimeStart,
                EndTime = c.TimeEnd
            }).ToList(),
        }).ToList();

        var ownerInfo = new OwnerResponseV2
        {
            OwnerId = query.Id,
            UserName = query.Account.UserName,
            WalletId = query.Account.Wallet.Id,
            PhoneNumber = query.Account.PhoneNumber,
            DateOfBirth = query.Account.DateOfBirth,
            Bio = query.Account.Bio,
            Court = listCourt,
            Address = query.Address,
        };

        return Task.FromResult(ownerInfo);
    }
}