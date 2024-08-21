using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities.CourtEntity.TimePeriod;
using MediatR;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BeatSportsAPI.Application.Features.Courts.TimePeriod.Command.CreateTimePeriod;
public class CreateTimePeriodHandler : IRequestHandler<CreateTimePeriodCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;
    public CreateTimePeriodHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _dbContext = beatSportsDbContext;
    }

    public async Task<BeatSportsResponse> Handle(CreateTimePeriodCommand request, CancellationToken cancellationToken)
    {
        // Đối với case chọn ngày 28 29 xong tạo tiếp 1 ngày 29 30 sẽ luôn false
        // bởi vì t đang lưu là ngày end sẽ là 23:59:59, nên khi gửi lại start = 29 thì nó sẽ bị trùng
        // 1. Xác định áp dụng cho ngày thường
        if (request.IsNormalDay)
        {
            /*
                    Sunday = 0,
                    Monday = 1,
                    Tuesday = 2,
                    Wednesday = 3,
                    Thursday = 4,
                    Friday = 5,
                    Saturday = 6
             */
            // Ghép chuỗi thứ lại với nhau
            string stringListDay = "";
            for (int i = 0; i < request.ListDayInWeek.Count; i++)
            {
                if (i < request.ListDayInWeek.Count - 1)
                {
                    stringListDay += request.ListDayInWeek[i] + ",";
                }
                else
                {
                    stringListDay += request.ListDayInWeek[i];
                }
            }
            // Kiểm tra xem với court id đó và setting id đó đang có những court sub
            // nào để add nó vào bảng nhiều nhiều
            var timePeriod = new Domain.Entities.CourtEntity.TimePeriod.TimePeriod
            {
                Description = request.Description,
                MinCancellationTime = request.MinCancellationTime,
                PriceAdjustment = request.PriceAdjustment,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                ListDayByString = stringListDay,
                IsNormalDay = true,
            };
            _dbContext.TimePeriods.Add(timePeriod);
            // Check dựa vào court setting trước
            foreach (var item in request.ListCourtSettingId)
            {
                var courtSubs = _dbContext.CourtSubdivisions.Where(x => x.CourtSubdivisionSettingId == item && 
                                                                        !x.IsDelete && 
                                                                        x.CourtId == request.CourtId &&
                                                                        x.CreatedStatus == Domain.Enums.CourtSubdivisionCreatedStatus.Accepted).ToList();
                foreach (var courtSub in courtSubs)
                {
                    var courtSubdivisionId = courtSub.Id;
                    // Kiểm tra trùng lặp khoảng thời gian
                    var timePeriods = (from tpcs in _dbContext.TimePeriodCourtSubdivisions
                                       join tp in _dbContext.TimePeriods on tpcs.TimePeriodId equals tp.Id
                                       join cs in _dbContext.CourtSubdivisions on tpcs.CourtSubdivisionId equals cs.Id
                                       where
                                       tpcs.CourtSubdivisionId == courtSubdivisionId
                                       && cs.CourtSubdivisionSettingId == item
                                       && tp.IsNormalDay
                                       && !tp.IsDelete
                                       && !cs.IsDelete
                                       select tp).ToList();
                    /*   if (timePeriods.Any(tp => !(request.EndTime <= tp.StartTime || request.StartTime >= tp.EndTime)))
                       {
                           throw new BadRequestException("Time period overlaps with existing period. Please check and try again.");
                       }*/
                    // Kiểm tra trùng lặp ngày và khung giờ
                    foreach (var existingPeriod in timePeriods)
                    {
                        // Normal day chắc chắc kh thể null ListDayByString được
                        var existingDays = existingPeriod.ListDayByString!.Split(',');
                        var newDays = stringListDay.Split(',');

                        // Kiểm tra nếu có bất kỳ ngày nào trùng
                        if (existingDays.Intersect(newDays).Any())
                        {
                            // Nếu ngày trùng, kiểm tra trùng lặp khung giờ
                            if (!(request.EndTime <= existingPeriod.StartTime || request.StartTime >= existingPeriod.EndTime))
                            {
                                throw new BadRequestException("Time period overlaps with existing period. Please check and try again.");
                            }
                        }
                    }

                    var newTimeperiodCourtSub = new TimePeriodCourtSubdivision
                    {
                        CourtSubdivisionId = courtSub.Id,
                        TimePeriodId = timePeriod.Id
                    };
                    _dbContext.TimePeriodCourtSubdivisions.Add(newTimeperiodCourtSub);
                }
            }
        }
        else if (request.DayEndTimePeriod != null && request.DayStartTimePeriod != null)
        {
            var flag = (DateTime)request.DayEndTimePeriod;
            var endDate = flag.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);
            var timePeriod = new Domain.Entities.CourtEntity.TimePeriod.TimePeriod
            {
                Description = request.Description,
                MinCancellationTime = request.MinCancellationTime,
                PriceAdjustment = request.PriceAdjustment,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                ListDayByString = null,
                IsNormalDay = false,
                StartDayApply = request.DayStartTimePeriod,
                EndDayApply = endDate,
            };
            _dbContext.TimePeriods.Add(timePeriod);
            foreach (var item in request.ListCourtSettingId)
            {
                var courtSubs = _dbContext.CourtSubdivisions.Where(x => x.CourtSubdivisionSettingId == item).ToList();
                foreach (var courtSub in courtSubs)
                {
                    var courtSubdivisionId = courtSub.Id;
                    // Kiểm tra trùng lặp khoảng thời gian
                    var timePeriods = (from tpcs in _dbContext.TimePeriodCourtSubdivisions
                                       join tp in _dbContext.TimePeriods on tpcs.TimePeriodId equals tp.Id
                                       join cs in _dbContext.CourtSubdivisions on tpcs.CourtSubdivisionId equals cs.Id
                                       where
                                       tpcs.CourtSubdivisionId == courtSubdivisionId
                                       && !tp.IsNormalDay
                                       && !tp.IsDelete
                                       && !cs.IsDelete
                                       select tp).ToList();
                    // Kiểm tra trùng lặp ngày
                    foreach (var existingPeriod in timePeriods)
                    {
                        if (request.DayStartTimePeriod <= existingPeriod.EndDayApply && endDate >= existingPeriod.StartDayApply)
                        {
                            // Nếu ngày trùng, kiểm tra trùng lặp khung giờ
                            if (!(request.EndTime <= existingPeriod.StartTime || request.StartTime >= existingPeriod.EndTime))
                            {
                                throw new BadRequestException("Time period overlaps with existing period. Please check and try again.");
                            }
                        }
                    }
                    var newTimeperiodCourtSub = new TimePeriodCourtSubdivision
                    {
                        CourtSubdivisionId = courtSub.Id,
                        TimePeriodId = timePeriod.Id
                    };
                    _dbContext.TimePeriodCourtSubdivisions.Add(newTimeperiodCourtSub);
                }
            }
        }
        await _dbContext.SaveChangesAsync();
        return new BeatSportsResponse
        {
            Message = "Create Time Period Successfully"
        };
    }
}