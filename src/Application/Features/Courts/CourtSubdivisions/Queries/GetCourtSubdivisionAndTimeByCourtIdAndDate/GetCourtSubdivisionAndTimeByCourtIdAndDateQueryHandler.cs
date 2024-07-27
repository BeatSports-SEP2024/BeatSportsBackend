using BeatSportsAPI.Application.Common.Exceptions;
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
        /*
         * public class CourtSubdivisionAndTime
{
    public string CourtId { get; set; } = null!;
    public string CourtName { get; set; } = null!;
    public List<ListCourtSubdivisionAndTimeDataByCourtSubdivisionId>? MiniCourt { get; set; }
}
public class ListCourtSubdivisionAndTimeDataByCourtSubdivisionId
{
    public string CourtSubdivisionId { get; set; } = null!;
    public string NameMiniCourt { get; set; } = null!;
    public List<ListTimeCheckingByCourtSubdivisionId>? TimeListBooked { get; set; }

}
public class ListTimeCheckingByCourtSubdivisionId
{
    public string TimeCheckingId { get; set; } = null!;
    public string StartTimeBooking { get; set; } = null!;
    public string EndBooking { get; set; } = null!;
}
         */
        var court = await _dbContext.Courts.Where(x => x.Id == request.CourtId && !x.IsDelete).SingleOrDefaultAsync();
        if (court == null)
        {
            throw new NotFoundException("Không tìm thấy court!");
        }
        var response = new CourtSubdivisionAndTime()
        {
            CourtId = court.Id.ToString(),
            CourtName = court.CourtName,
            MiniCourt = new List<ListCourtSubdivisionAndTimeDataByCourtSubdivisionId>()
        };

        var listCourtSubdivision = await _dbContext.CourtSubdivisions.Where(x => x.CourtId == request.CourtId && !x.IsDelete).OrderBy(X => X.CourtSubdivisionName).ToListAsync();
        foreach (var item in listCourtSubdivision)
        {
            var newMinicourt = new ListCourtSubdivisionAndTimeDataByCourtSubdivisionId()
            {
                CourtSubdivisionId = item.Id.ToString(),
                NameMiniCourt = item.CourtSubdivisionName,
                TimeListBooked = new List<ListTimeCheckingByCourtSubdivisionId>()
            };
            var listTimecheck = await _dbContext.TimeChecking.Where(x => x.CourtSubdivisionId == item.Id && (x.StartTime.Year == request.DateCheck.Year
                                                  && x.StartTime.Month == request.DateCheck.Month
                                                  && x.StartTime.Day == request.DateCheck.Day)).ToListAsync();
            foreach (var timecheck in listTimecheck)
            {
                var newTimeCheck = new ListTimeCheckingByCourtSubdivisionId
                {
                    TimeCheckingId = timecheck.Id.ToString(),
                    StartTimeBooking = timecheck.StartTime.ToString("HH:mm"),
                    EndBooking = timecheck.EndTime.ToString("HH:mm")
                };
                newMinicourt.TimeListBooked.Add(newTimeCheck);
            }
            response.MiniCourt.Add(newMinicourt);
        }
        /*
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
                                    }).SingleOrDefaultAsync();*/
        return response;
    }
}
