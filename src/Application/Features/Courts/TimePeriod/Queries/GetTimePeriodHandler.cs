using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;
using BeatSportsAPI.Domain.Entities.CourtEntity.TimePeriod;
using StackExchange.Redis;
using BeatSportsAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Courts.TimePeriod.Queries;

public class GetTimePeriodHandler : IRequestHandler<GetTimePeriodCommand, List<TimePeriodResponse>>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetTimePeriodHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _dbContext = beatSportsDbContext;
        _mapper = mapper;
    }

    public async Task<List<TimePeriodResponse>> Handle(GetTimePeriodCommand request, CancellationToken cancellationToken)
    {
        var response = await (from tp in _dbContext.TimePeriods
                              join tpcs in _dbContext.TimePeriodCourtSubdivisions on tp.Id equals tpcs.TimePeriodId
                              join cs in _dbContext.CourtSubdivisions on tpcs.CourtSubdivisionId equals cs.Id
                              join css in _dbContext.CourtSubdivisionSettings on cs.CourtSubdivisionSettingId equals css.Id
                              join sc in _dbContext.SportsCategories on css.SportCategoryId equals sc.Id
                              join c in _dbContext.Courts on cs.CourtId equals c.Id
                              where
                              c.Id == request.CourtId &&
                              sc.Id == request.SportCategoryId &&
                              !tp.IsDelete &&
                              !cs.IsDelete &&
                              !css.IsDelete &&
                              !sc.IsDelete &&
                              !c.IsDelete &&
                              (tp.StartDayApply.HasValue && tp.EndDayApply.HasValue ? tp.StartDayApply >= DateTime.Now || tp.EndDayApply >= DateTime.Now : true)
                              group new { tp, css, sc } by new
                              {
                                  TimePeriodId = tp.Id,
                                  tp.IsNormalDay,
                                  tp.StartDayApply,
                                  CourtSubdivisionSettingId = css.Id,
                                  css.CourtType,
                                  tp.MinCancellationTime,
                                  tp.Description,
                                  tp.StartTime,
                                  tp.EndTime,
                                  tp.EndDayApply,
                                  tp.ListDayByString,
                                  tp.PriceAdjustment
                              } into g
                              orderby g.Key.IsNormalDay, g.Key.CourtType, g.Key.StartDayApply
                              select new TimePeriodResponse
                              {
                                  TimePeriodId = g.Key.TimePeriodId,
                                  CourtSubdivisionSettingId = g.Key.CourtSubdivisionSettingId,
                                  CourtSubdivisionSettingType = g.Key.CourtType,
                                  MinCancellationTime = g.Key.MinCancellationTime.ToString(@"hh\:mm\:ss"),
                                  Description = g.Key.Description,
                                  StartTime = g.Key.StartTime.ToString(@"hh\:mm\:ss"),
                                  EndTime = g.Key.EndTime.ToString(@"hh\:mm\:ss"),
                                  IsNormalDay = g.Key.IsNormalDay,
                                  DayStartApply = g.Key.StartDayApply.HasValue ? g.Key.StartDayApply.Value.ToString("yyyy-MM-dd") : null,
                                  DayEndApply = g.Key.EndDayApply.HasValue ? g.Key.EndDayApply.Value.ToString("yyyy-MM-dd") : null,
                                  StringListDayInWeekApply = g.Key.ListDayByString,
                                  PriceAdjustment = (decimal)g.Key.PriceAdjustment
                              }).ToListAsync();

        foreach (var item in response)
        {
            item.StringListDayInWeekApplyDescription = ProcessRequest(item.StringListDayInWeekApply);
        }
        return response;
    }
    private string? ProcessRequest(string? request)
    {
        if (request == null)
        {
            return null;
        }

        var daysMapping = new Dictionary<string, string>
        {
            { "0", "CN" },
            { "1", "T2" },
            { "2", "T3" },
            { "3", "T4" },
            { "4", "T5" },
            { "5", "T6" },
            { "6", "T7" }
        };
        var dayParts = request.Split(',');
        var days = dayParts.Select(day => daysMapping.ContainsKey(day) ? daysMapping[day] : day).ToArray();

        return "Áp dụng vào các ngày: " + string.Join(",", days) + " trong tuần";
    }
}