using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingDetailReadyForFinishBooking;
public class GetBookingDetailReadyForFinishBookingQueryHandler : IRequestHandler<GetBookingDetailReadyForFinishBookingQuery, BookingDetailReadyForFinishBookingResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public GetBookingDetailReadyForFinishBookingQueryHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BookingDetailReadyForFinishBookingResponse> Handle(GetBookingDetailReadyForFinishBookingQuery request, CancellationToken cancellationToken)
    {
        // B1. Kiểm tra xem sân nhỏ muốn đặt tại thời điểm muốn chơi có trùng với time checking hay kh ?
        var courtSub = await _dbContext.CourtSubdivisions.Where(x => x.Id == request.CourtSubdivisionId).SingleOrDefaultAsync();
        if (courtSub == null)
        {
            throw new NotFoundException("Không tìm thấy sân nào có Id: " + request.CourtSubdivisionId);
        }
        var dateTimeStartTimeWantToPlay = request.DayWantToPlay + request.StartTimeWantToPlay;
        var dateTimeEndTimeWantToPlay = request.DayWantToPlay + request.EndTimeWantToPlay;
        // Giờ dô time check check nè
        var timeCheckFlag = await _dbContext.TimeChecking.Where(x => x.CourtSubdivisionId == request.CourtSubdivisionId && !x.IsDelete &&
                                                                     (
                                                                     (dateTimeStartTimeWantToPlay <= x.EndTime && dateTimeStartTimeWantToPlay >= x.EndTime) ||
                                                                     (dateTimeEndTimeWantToPlay <= x.EndTime && dateTimeEndTimeWantToPlay >= x.EndTime) ||
                                                                     dateTimeStartTimeWantToPlay <= x.StartTime && dateTimeEndTimeWantToPlay >= x.EndTime)
                                                                     ).ToListAsync();
        if (timeCheckFlag.Any())
        {
            throw new BadRequestException("Sân bạn muốn đặt đã được đặt trước đó! Vui lòng lựa sân khác nhé");
        }
        // B2. Nếu OK rồi thì dựa vào court id trong court sub để tìm trong bảng time period
        // - Mục đích là kiếm xem sân này có nhiều khung giờ hay kh
        // - Nếu có nhiều khung giờ thì check xem thời gian muốn đặt nằm ở trong khung giờ nào để tính tiền
        var timePeriods = await _dbContext.TimePeriods.Where(x => x.CourtId == courtSub.CourtId && !x.IsDelete).ToListAsync();
        var court = await _dbContext.Courts.Where(x => x.Id == courtSub.CourtId).SingleOrDefaultAsync();
        var response = new BookingDetailReadyForFinishBookingResponse()
        {
            CourtId = courtSub.CourtId,
            CourtName = court.CourtName,
            ImageCourtURL = court.CourtAvatarImgUrls,
        };
        var listCourtSubInReponse = new List<CourtDetailInBookingDetailReadyForFinishBookingReponse>();
        decimal totalPrice = 0;
        TimeSpan currentStart = request.StartTimeWantToPlay;
        if (timePeriods.Any())
        {
            // Tổng số giời chơi
            TimeSpan totalTimeWantToPlay = request.EndTimeWantToPlay - request.StartTimeWantToPlay;
            foreach (var item in timePeriods)
            {
                TimeSpan periodStart = item.StartTime;
                TimeSpan periodEnd = item.EndTime;

                // Nếu currentStart nhỏ hơn periodStart và periodStart nhỏ hơn timeEnd
                // Vd: 15(cS) < 17(ps) && 17(ps) < 23(te)
                if (currentStart < periodStart && periodStart < request.EndTimeWantToPlay)
                {
                    // Nếu vào điều kiện này có nghĩa là chưa bắt đầu time period
                    var newCourtSubResponse = new CourtDetailInBookingDetailReadyForFinishBookingReponse()
                    {
                        // Do chưa vào có nghĩa là null, và giá cũng là giá gốc
                        TimePeriodId = null,
                        PriceInTimePeriod = courtSub.BasePrice,
                        TimePeriodDescription = "Khung giờ thường từ "
                        + currentStart.ToString(@"hh\:mm")
                        + " đến "
                        + periodStart.ToString(@"hh\:mm")
                    };
                    var timePlayInThisPeriod = periodStart - currentStart;
                    listCourtSubInReponse.Add(newCourtSubResponse);
                    totalPrice += courtSub.BasePrice * Convert.ToDecimal(timePlayInThisPeriod.TotalHours);
                    currentStart = periodStart;
                }
                // Nếu currentStart nhỏ hơn periodEnd và periodEnd nhỏ hơn timeEnd
                if (currentStart < periodEnd && periodEnd < request.EndTimeWantToPlay)
                {
                    var newCourtSubResponse = new CourtDetailInBookingDetailReadyForFinishBookingReponse()
                    {
                        TimePeriodId = item.Id.ToString(),
                        PriceInTimePeriod = courtSub.BasePrice * item.RateMultiplier ?? 1,
                        TimePeriodDescription = "Khung giờ " 
                        + "'"
                        + item.Description
                        + "'"
                        + " từ "
                        + currentStart.ToString(@"hh\:mm")
                        + " đến "
                        + periodEnd.ToString(@"hh\:mm")
                    };
                    var timePlayInThisPeriod = periodEnd - currentStart;
                    listCourtSubInReponse.Add(newCourtSubResponse);

                    totalPrice += courtSub.BasePrice * Convert.ToDecimal(timePlayInThisPeriod.TotalHours) * item.RateMultiplier ?? 1;
                    currentStart = periodEnd;
                }
                // Nếu kết thúc trong khung giờ luôn
                else if (currentStart < periodEnd && request.EndTimeWantToPlay < periodEnd)
                {
                    var newCourtSubResponse = new CourtDetailInBookingDetailReadyForFinishBookingReponse()
                    {
                        TimePeriodId = item.Id.ToString(),
                        PriceInTimePeriod = courtSub.BasePrice * item.RateMultiplier ?? 1,
                        TimePeriodDescription = "Khung giờ thường từ "
                        + currentStart.ToString(@"hh\:mm")
                        + " đến "
                        + request.EndTimeWantToPlay.ToString(@"hh\:mm")
                    };
                    var timePlayInThisPeriod = request.EndTimeWantToPlay - currentStart;
                    listCourtSubInReponse.Add(newCourtSubResponse);

                    totalPrice += courtSub.BasePrice * Convert.ToDecimal(timePlayInThisPeriod.TotalHours) * item.RateMultiplier ?? 1;
                    currentStart = periodEnd;
                }
            }
            // Thêm đoạn thời gian còn lại sau khoảng thời gian đặc biệt
            if (currentStart < request.EndTimeWantToPlay)
            {
                var newCourtSubResponse = new CourtDetailInBookingDetailReadyForFinishBookingReponse()
                {
                    TimePeriodId = null,
                    PriceInTimePeriod = courtSub.BasePrice,
                    TimePeriodDescription = "Khung giờ từ "
                                        + currentStart.ToString(@"hh\:mm")
                                        + " đến "
                                        + request.EndTimeWantToPlay.ToString(@"hh\:mm")
                };
                listCourtSubInReponse.Add(newCourtSubResponse);

                var timePlayInThisPeriod = request.EndTimeWantToPlay - currentStart;
                totalPrice += courtSub.BasePrice * Convert.ToDecimal(timePlayInThisPeriod.TotalHours);
                currentStart = request.EndTimeWantToPlay;
            }
        }
        response.ListCourtByTimePeriod = listCourtSubInReponse;
        response.TotalPrice = Math.Round(totalPrice, 2);
        return response;
    }
}
