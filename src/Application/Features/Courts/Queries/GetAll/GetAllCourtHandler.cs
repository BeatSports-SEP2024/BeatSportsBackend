using AutoFilterer.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetAll;
public class GetAllCourtHandler : IRequestHandler<GetAllCourtCommand, PaginatedList<CourtResponseV2>>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetAllCourtHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public Task<PaginatedList<CourtResponseV2>> Handle(GetAllCourtCommand request, CancellationToken cancellationToken)
    {
        if (request.PageIndex <= 0 || request.PageSize <= 0)
        {
            throw new BadRequestException("Page index and page size cannot less than 0");
        }

        if (request.KeyWords == null)
        {
            request.KeyWords = "";
        }

        IQueryable<Court> query = _dbContext.Courts
            .Where(x => !x.IsDelete && (x.CourtName.Contains(request.KeyWords) || x.Address.Contains(request.KeyWords)))
            .OrderByDescending(b => b.Created)
            .Include(x => x.Owner).ThenInclude(x => x.Account)
            .Include(x => x.CourtSubdivision);

        if (request.StartDate.HasValue && request.EndDate.HasValue)
        {
            query = query.Where(tp => tp.Created.Date >= request.StartDate.Value.Date && tp.Created.Date <= request.EndDate.Value.Date);
        }
        else if (request.StartDate.HasValue)
        {
            query = query.Where(tp => tp.Created.Date >= request.StartDate.Value.Date);
        }
        else if (request.EndDate.HasValue)
        {
            query = query.Where(tp => tp.Created.Date <= request.EndDate.Value.Date);
        }

        query = query.OrderByDescending(tp => tp.Created);

        var list = query.Select(c => new CourtResponseV2
        {
            Id = c.Id,
            OwnerName = c.Owner.Account.FirstName + " " + c.Owner.Account.LastName,
            CourtName = c.CourtName,
            Description = c.Description,
            Address = c.Address,
            LatitudeDelta = c.LatitudeDelta,
            LongitudeDelta = c.LongitudeDelta,
            GoogleMapURLs = c.GoogleMapURLs,
            TimeStart = c.TimeStart,
            TimeEnd = c.TimeEnd,
            Created = c.Created,
            IsDelete = c.IsDelete,
        })
        .PaginatedListAsync(request.PageIndex, request.PageSize);

        return list;
    }
}
