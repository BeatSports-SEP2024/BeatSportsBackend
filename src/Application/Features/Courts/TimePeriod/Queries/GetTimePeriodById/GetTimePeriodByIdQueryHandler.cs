/*using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Courts.TimePeriod.Queries.GetTimePeriodById;
public class GetTimePeriodByIdQueryHandler : IRequestHandler<GetTimePeriodByIdQuery, TimePeriodWithCourtInformationResponse>
{
    private IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetTimePeriodByIdQueryHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<TimePeriodWithCourtInformationResponse> Handle(GetTimePeriodByIdQuery request, CancellationToken cancellationToken)
    {
        var timeperiod = await _dbContext.TimePeriods.Where(x => x.Id == request.TimePeriodId && !x.IsDelete).Include(x => x.Courts).SingleOrDefaultAsync();
        var response = new TimePeriodWithCourtInformationResponse()
        {
            Address = timeperiod.Courts.Address,
            CourtId = timeperiod.Courts.Id,
            CourtName = timeperiod.Courts.CourtName,
            Description = timeperiod.Description,
            EndTime = timeperiod.EndTime,
            MinCancellationTime = timeperiod.MinCancellationTime,
            RateMultiplier = timeperiod.RateMultiplier ?? 1,
            StartTime = timeperiod.StartTime,
            TimePeriodId = timeperiod.Id,
        };
        return response;
    }
}
*/