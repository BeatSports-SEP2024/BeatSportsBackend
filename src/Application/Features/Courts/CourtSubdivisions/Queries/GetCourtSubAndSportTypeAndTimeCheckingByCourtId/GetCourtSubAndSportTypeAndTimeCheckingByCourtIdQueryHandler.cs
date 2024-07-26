using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetCourtSubdivisionAndTimeByCourtIdAndDate;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetCourtSubAndSportTypeAndTimeCheckingByCourtId;
public class GetCourtSubAndSportTypeAndTimeCheckingByCourtIdQueryHandler : IRequestHandler<GetCourtSubAndCourtSettingsAndTimeChecking, ResponseCourtDataInCourtSubAndCourtSettingsAndTimeChecking?>
{
    private readonly IBeatSportsDbContext _dbContext;

    public GetCourtSubAndSportTypeAndTimeCheckingByCourtIdQueryHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<ResponseCourtDataInCourtSubAndCourtSettingsAndTimeChecking?> Handle(GetCourtSubAndCourtSettingsAndTimeChecking request, CancellationToken cancellationToken)
    {
        var result = await (from c in _dbContext.Courts
                            where c.Id == request.CourtId
                            select new ResponseCourtDataInCourtSubAndCourtSettingsAndTimeChecking
                            {
                                CourtId = c.Id,
                                TimeStart = c.TimeStart.ToString(@"hh\:mm\:ss"),
                                TimeEnd = c.TimeEnd.ToString(@"hh\:mm\:ss"),
                                CourtSubSettingResponses = (from cs in _dbContext.CourtSubdivisions
                                                            join css in _dbContext.CourtSubdivisionSettings
                                                            on cs.CourtSubdivisionSettingId equals css.Id into courtSubAndSettingsGroup

                                                            from courtSubSetting in courtSubAndSettingsGroup.DefaultIfEmpty()
                                                            join sc in _dbContext.SportsCategories
                                                            on courtSubSetting.SportCategoryId equals sc.Id into courtSubSettingAndCategoryGroup

                                                            from sportCategory in courtSubSettingAndCategoryGroup.DefaultIfEmpty()

                                                            where cs.CourtId == request.CourtId
                                                            && cs.IsActive
                                                            && cs.CreatedStatus != CourtSubdivisionCreatedStatus.Pending
                                                            && cs.CreatedStatus != CourtSubdivisionCreatedStatus.Rejected

                                                            select new
                                                            {
                                                                courtSubSetting.Id,
                                                                SportCategoryId = sportCategory.Id,
                                                                SportCategoryName = sportCategory.Name,
                                                                CourtType = courtSubSetting.CourtType,
                                                                ShortName = courtSubSetting.ShortName,
                                                                CourtSubdivisionId = cs.Id,
                                                                CourtSubdivisionName = cs.CourtSubdivisionName
                                                            } into courtSubSettings
                                                            group courtSubSettings by new { courtSubSettings.Id, courtSubSettings.SportCategoryId, courtSubSettings.SportCategoryName, courtSubSettings.CourtType, courtSubSettings.ShortName } into groupedCourtSubSettings
                                                            select new CourtSubSettingResponseV2
                                                            {
                                                                CourtSubSettingId = groupedCourtSubSettings.Key.Id,
                                                                SportCategoryId = groupedCourtSubSettings.Key.SportCategoryId,
                                                                SportCategoryName = groupedCourtSubSettings.Key.SportCategoryName,
                                                                TypeSize = groupedCourtSubSettings.Key.CourtType,
                                                                ShortName = groupedCourtSubSettings.Key.ShortName,
                                                                CourtSubdivision = groupedCourtSubSettings.Select(sub => new ListCourtSubdivisionAndTimeDataByCourtSubdivisionId
                                                                {
                                                                    CourtSubdivisionId = sub.CourtSubdivisionId.ToString(),
                                                                    NameMiniCourt = sub.CourtSubdivisionName,
                                                                    TimeListBooked = (from timeChecking in _dbContext.TimeChecking
                                                                                      where timeChecking.CourtSubdivisionId == sub.CourtSubdivisionId
                                                                                      && (timeChecking.StartTime.Year == request.DateCheck.Year
                                                                                          && timeChecking.StartTime.Month == request.DateCheck.Month
                                                                                          && timeChecking.StartTime.Day == request.DateCheck.Day)
                                                                                      select new ListTimeCheckingByCourtSubdivisionId
                                                                                      {
                                                                                          TimeCheckingId = timeChecking.Id.ToString(),
                                                                                          StartTimeBooking = timeChecking.StartTime.ToString("HH:mm"),
                                                                                          EndBooking = timeChecking.EndTime.ToString("HH:mm")
                                                                                      }).ToList()
                                                                }).ToList()
                                                            }).ToList()
                            }).FirstOrDefaultAsync();

        return result;
    }

    /*    public async Task<ResponseCourtDataInCourtSubAndCourtSettingsAndTimeChecking?> Handle(GetCourtSubAndCourtSettingsAndTimeChecking request, CancellationToken cancellationToken)
        {
            var result = await (from c in _dbContext.Courts
                                where c.Id == request.CourtId
                                select new ResponseCourtDataInCourtSubAndCourtSettingsAndTimeChecking
                                {
                                    CourtId = c.Id,
                                    TimeStart = c.TimeStart.ToString(@"hh\:mm\:ss"),
                                    TimeEnd = c.TimeEnd.ToString(@"hh\:mm\:ss"),
                                    CourtSubSettingResponses = (from cs in _dbContext.CourtSubdivisions
                                                                join css in _dbContext.CourtSubdivisionSettings
                                                                on cs.CourtSubdivisionSettingId equals css.Id into courtSubAndSettingsGroup

                                                                from courtSubSetting in courtSubAndSettingsGroup.DefaultIfEmpty()
                                                                join sc in _dbContext.SportsCategories
                                                                on courtSubSetting.SportCategoryId equals sc.Id into courtSubSettingAndCategoryGroup

                                                                from sportCategory in courtSubSettingAndCategoryGroup.DefaultIfEmpty()

                                                                where cs.CourtId == request.CourtId
                                                                && cs.IsActive
                                                                && cs.CreatedStatus != CourtSubdivisionCreatedStatus.Pending
                                                                && cs.CreatedStatus != CourtSubdivisionCreatedStatus.Rejected

                                                                select new CourtSubSettingResponseV2
                                                                {
                                                                    CourtSubSettingId = courtSubSetting.Id,
                                                                    SportCategoryId = sportCategory.Id,
                                                                    SportCategoryName = sportCategory.Name,
                                                                    TypeSize = courtSubSetting.CourtType,
                                                                    CourtSubdivision = (from sub in _dbContext.CourtSubdivisions
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
                                                                }).ToList()
                                }).FirstOrDefaultAsync();
            return result;
        }
    */
}
