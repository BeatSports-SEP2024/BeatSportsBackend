using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetAllCourtSubdivisionPending;
public class GetAllCourtSubdivisionPendingHandler : IRequestHandler<GetAllCourtSubdivisionPendingCommand, PaginatedList<CourtSubdivisionResponseV3>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public GetAllCourtSubdivisionPendingHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public Task<PaginatedList<CourtSubdivisionResponseV3>> Handle(GetAllCourtSubdivisionPendingCommand request, CancellationToken cancellationToken)
    {
        var query = _beatSportsDbContext.CourtSubdivisions
            .Where(c => !c.IsDelete && c.CourtId == request.CourtId);

        var checkPending = query.Where(c => c.CreatedStatus == CourtSubdivisionCreatedStatus.Pending);

        var listPending = checkPending.Select(x => new CourtSubdivisionResponseV3 
        {
            Id = x.Id,
            CourtId = x.Court.Id,
            CourtName = x.Court.CourtName,
            Description = x.CourtSubdivisionDescription,
            ImageURL = ImageUrlSplitter.SplitAndGetFirstImageUrls(x.Court.ImageUrls),
            IsActive = x.IsActive,
            BasePrice = x.BasePrice,
            CourtSubdivisionName = x.CourtSubdivisionName,
            Status = x.CreatedStatus.ToString(),
        }).PaginatedListAsync(request.PageIndex, request.PageSize);

        return listPending;
    }
}