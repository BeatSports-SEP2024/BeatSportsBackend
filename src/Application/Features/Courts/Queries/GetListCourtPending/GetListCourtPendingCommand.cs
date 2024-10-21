using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetListCourtPending;
public class GetListCourtPendingCommand : IRequest<PaginatedList<CourtResponseV6>>
{
    [Required]
    public int PageIndex { get; set; }
    [Required]
    public int PageSize { get; set; }
    public string KeyWords { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class GetListCourtPendingCommandHandler : IRequestHandler<GetListCourtPendingCommand, PaginatedList<CourtResponseV6>>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetListCourtPendingCommandHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public async Task<PaginatedList<CourtResponseV6>> Handle(GetListCourtPendingCommand request, CancellationToken cancellationToken)
    {
        var keyWords = request.KeyWords ?? string.Empty;

        IQueryable<Court> query = _dbContext.Courts
            .Where(x => !x.IsDelete &&
                        x.CourtSubdivision.Any(cs => cs.CreatedStatus == Domain.Enums.CourtSubdivisionCreatedStatus.Pending) &&
                        (x.CourtName.Contains(keyWords) || x.Address.Contains(keyWords)))
            .OrderByDescending(b => b.Created)
            .Include(x => x.Owner).ThenInclude(x => x.Account)
            .Include(x => x.CourtSubdivision);

        if (request.StartDate.HasValue)
        {
            query = query.Where(tp => tp.Created.Date >= request.StartDate.Value.Date);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(tp => tp.Created.Date <= request.EndDate.Value.Date);
        }

        var list = await query.Select(c => new CourtResponseV6
        {
            Id = c.Id,
            OwnerName = c.Owner.Account.FirstName + " " + c.Owner.Account.LastName,
            CourtName = c.CourtName,
            Description = c.Description,
            Address = c.Address,
            LatitudeDelta = c.LatitudeDelta,
            LongitudeDelta = c.LongitudeDelta,
            Latitude = c.Latitude,
            Longitude = c.Longitude,
            GoogleMapURLs = c.GoogleMapURLs,
            TimeStart = c.TimeStart,
            TimeEnd = c.TimeEnd,
            Created = c.Created,
            IsDelete = c.IsDelete,
            StatusCourtSubdivision = Domain.Enums.CourtSubdivisionCreatedStatus.Pending,
        })
        .PaginatedListAsync(request.PageIndex, request.PageSize);

        return list;
    }

}
