using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Utils.Extensions;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetCourtSubdivisionAndTimeByCourtIdAndDate;
public class GetCourtSubdivisionAndTimeByCourtIdAndDateQueryHandler : IRequestHandler<GetCourtSubdivisionAndTimeByCourtIdAndDateQuery, CourtSubdivisionAndTime>
{
    private IBeatSportsDbContext _dbContext;

    public GetCourtSubdivisionAndTimeByCourtIdAndDateQueryHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CourtSubdivisionAndTime?> Handle(GetCourtSubdivisionAndTimeByCourtIdAndDateQuery request, CancellationToken cancellationToken)
    {
        var result = await (from court in _dbContext.Courts
                            where court.Id == request.CourtId
                            select new CourtSubdivisionAndTime
                            {
                                CourtId = court.Id.ToString(),
                                CourtName = court.CourtName,
                                MiniCourt = (from sub in _dbContext.CourtSubdivisions
                                             join timeChecking in _dbContext.TimeChecking
                                             on sub.Id equals timeChecking.CourtSubdivisionId into timeCheckingGroup
                                             from timeChecking in timeCheckingGroup.DefaultIfEmpty()
                                             where sub.CourtId == request.CourtId
                                             && sub.CreatedStatus != CourtSubdivisionCreatedStatus.Pending
                                             && sub.CreatedStatus != CourtSubdivisionCreatedStatus.Rejected
                                             && (timeChecking == null ||
                                                 (timeChecking.StartTime.Year == request.DateCheck.Year
                                                  && timeChecking.StartTime.Month == request.DateCheck.Month
                                                  && timeChecking.StartTime.Day == request.DateCheck.Day))
                                             select new ListCourtSubdivisionAndTimeDataByCourtSubdivisionId
                                             {
                                                 CourtSubdivisionId = sub.Id.ToString(),
                                                 NameMiniCourt = sub.CourtSubdivisionName,
                                                 TimeListBooked = (timeChecking == null) ? new List<ListTimeCheckingByCourtSubdivisionId>() :
                                                    new List<ListTimeCheckingByCourtSubdivisionId>
                                                    {
                                                    new ListTimeCheckingByCourtSubdivisionId
                                                    {
                                                        TimeCheckingId = timeChecking.Id.ToString(),
                                                        StartTimeBooking = timeChecking.StartTime.ToString("HH:mm"),
                                                        EndBooking = timeChecking.EndTime.ToString("HH:mm")
                                                    }
                                                    }
                                             }).ToList()
                            }).SingleOrDefaultAsync();
        return result;
    }
}
