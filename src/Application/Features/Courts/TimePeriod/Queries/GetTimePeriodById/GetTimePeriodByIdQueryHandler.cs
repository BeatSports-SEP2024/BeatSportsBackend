using System.Net.WebSockets;
using AutoMapper;
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
        /*
         * 
         * 

 select cs.Id,cs.CourtSubdivisionName, cs.CourtId from TimePeriods as TP
 join TimePeriodCourtSubdivisions as tpcs
 on tp.Id = tpcs.TimePeriodId

 join CourtSubdivisions as cs
 on tpcs.CourtSubdivisionId = cs.Id

 group by cs.Id,cs.CourtSubdivisionName, cs.CourtId
 order by CourtSubdivisionName



         */

        var listCourtSubInTimePeriodWithCourtInformationResponse = await (from tp in _dbContext.TimePeriods
                                                                          join tpcs in _dbContext.TimePeriodCourtSubdivisions
                                                                          on tp.Id equals tpcs.TimePeriodId

                                                                          join cs in _dbContext.CourtSubdivisions
                                                                          on tpcs.CourtSubdivisionId equals cs.Id

                                                                          where
                                                                          tp.Id == request.TimePeriodId &&
                                                                          !tp.IsDelete &&
                                                                          !cs.IsDelete

                                                                          group new { cs } by new
                                                                          {
                                                                              cs.Id,
                                                                              cs.CourtSubdivisionName,
                                                                              cs.CourtId
                                                                          } into g
                                                                          orderby g.Key.CourtSubdivisionName
                                                                          select new CourtSubInTimePeriodWithCourtInformationResponse
                                                                          {
                                                                              CourtSubdivisionId = g.Key.Id,
                                                                              CourtSubdivisionName = g.Key.CourtSubdivisionName,
                                                                              CourtId = g.Key.CourtId,
                                                                          }).ToListAsync();
        if (listCourtSubInTimePeriodWithCourtInformationResponse.Any())
        {
            // Vì mặc dù time period có thể khác ID nhưng khi apply thì chỉ có thể tạo từ sân lớn
            // Nên list này luôn cùng Id với sân lớn với 1 time period id 
            var court = await _dbContext.Courts.Where(x => x.Id == listCourtSubInTimePeriodWithCourtInformationResponse[0].CourtId && !x.IsDelete).SingleOrDefaultAsync();
            var timepriod = await _dbContext.TimePeriods.Where(x => x.Id == request.TimePeriodId).SingleOrDefaultAsync();
            var response = new TimePeriodWithCourtInformationResponse()
            {
                TimePeriodId = timepriod.Id,
                Address = court.Address,
                CourtId = court.Id,
                CourtName = court.CourtName,
                DayEndApply = timepriod.EndDayApply.HasValue ? timepriod.EndDayApply.Value.ToString("yyyy-MM-dd") : null,
                DayStartApply = timepriod.StartDayApply.HasValue ? timepriod.StartDayApply.Value.ToString("yyyy-MM-dd") : null,
                Description = timepriod.Description,
                StartTime = timepriod.StartTime.ToString(@"hh\:mm\:ss"),
                EndTime = timepriod.EndTime.ToString(@"hh\:mm\:ss"),
                IsNormalDay = timepriod.IsNormalDay,
                ListCourtSub = listCourtSubInTimePeriodWithCourtInformationResponse,
                MinCancellationTime = timepriod.MinCancellationTime.ToString(@"hh\:mm\:ss"),
                PriceAdjustment = (decimal)timepriod.PriceAdjustment,
                StringListDayInWeekApply = timepriod.ListDayByString,
            };
            response.StringListDayInWeekApplyDescription = ProcessRequest(response.StringListDayInWeekApply);
            return response;

        }
        return null;
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
