using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.TimePeriod.Queries;

public class GetTimePeriodHandler : IRequestHandler<GetTimePeriodCommand, PaginatedList<TimePeriodResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;

    public GetTimePeriodHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }

    public async Task<PaginatedList<TimePeriodResponse>> Handle(GetTimePeriodCommand request, CancellationToken cancellationToken)
    {
        if (request.PageIndex <= 0 || request.PageSize <= 0)
        {
            throw new BadRequestException("Page index and page size cannot be less than 0");
        }

        var query = _beatSportsDbContext.TimePeriods
            .Where(tp => !tp.IsDelete);

        query = query.OrderBy(tp => tp.StartTime);

        var response = await query
            .ProjectTo<TimePeriodResponse>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageIndex, request.PageSize);

        return response;
    }
}