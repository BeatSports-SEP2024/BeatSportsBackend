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
    public Task<PaginatedList<CourtResponseV6>> Handle(GetListCourtPendingCommand request, CancellationToken cancellationToken)
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
            .Where(x => !x.IsDelete &&
                        x.CourtSubdivision.Any(cs => cs.CreatedStatus == "Pending") &&
                        (x.CourtName.Contains(request.KeyWords) || x.Address.Contains(request.KeyWords)))
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

        var list = query.Select(c => new CourtResponseV6
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
            StatusCourtSubdivision = "Pending",
        })
        .PaginatedListAsync(request.PageIndex, request.PageSize);

        return list;
    }
}
