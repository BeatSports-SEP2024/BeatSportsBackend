﻿using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Services.Redis;

namespace BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingDetailReadyForFinishBooking;
public class GetBookingDetailReadyForFinishBookingQueryHandler : IRequestHandler<GetBookingDetailReadyForFinishBookingQuery, BookingDetailReadyForFinishBookingResponse>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IDatabase _database;
    public GetBookingDetailReadyForFinishBookingQueryHandler(IBeatSportsDbContext dbContext, IDatabase database)
    {
        _dbContext = dbContext;
        _database = database;
    }

    public async Task<BookingDetailReadyForFinishBookingResponse> Handle(GetBookingDetailReadyForFinishBookingQuery request, CancellationToken cancellationToken)
    {
        // B0. Tạo khóa
        string courtSubdivisionId = request.CourtSubdivisionId.ToString();
        string dayWantToPlay = request.DayWantToPlay.ToString("yyyyMMdd");
        string startTimeWantToPlay = request.StartTimeWantToPlay.ToString(@"hh\:mm");
        string lockKey = $"booking:{courtSubdivisionId}:{dayWantToPlay}:{startTimeWantToPlay}:lock";
        string lockValue = Guid.NewGuid().ToString();
        TimeSpan expiry = TimeSpan.FromMinutes(2); // Khóa trong 2 phút
        // B1. Kiểm tra xem sân nhỏ muốn đặt tại thời điểm muốn chơi có trùng với time checking hay không?
        using (var redisLock = new RedisLock(_database, lockKey, lockValue, expiry))
        {
            if (redisLock.AcquireLock())
            {
                try
                {
                    // B1. Kiểm tra xem sân nhỏ muốn đặt tại thời điểm muốn chơi có trùng với time checking hay không?
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
                                                                         (dateTimeStartTimeWantToPlay <= x.EndTime && dateTimeStartTimeWantToPlay >= x.StartTime) ||
                                                                         (dateTimeEndTimeWantToPlay <= x.EndTime && dateTimeEndTimeWantToPlay >= x.StartTime) ||
                                                                         (dateTimeStartTimeWantToPlay <= x.StartTime && dateTimeEndTimeWantToPlay >= x.EndTime)
                                                                         )).ToListAsync();
                    if (timeCheckFlag.Any())
                    {
                        throw new BadRequestException("Sân bạn muốn đặt đã được đặt trước đó! Vui lòng lựa sân khác nhé");
                    }

                    // B2. Lấy các TimePeriods áp dụng cho CourtSubdivisionId
                    var timePeriods = await (from tp in _dbContext.TimePeriods
                                             join tpcs in _dbContext.TimePeriodCourtSubdivisions
                                             on tp.Id equals tpcs.TimePeriodId
                                             where tpcs.CourtSubdivisionId == request.CourtSubdivisionId
                                             orderby tp.IsNormalDay, tp.StartDayApply
                                             select tp).ToListAsync();

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

                    // Kiểm tra các TimePeriods có IsNormalDay = false trước
                    var specialDayPeriods = timePeriods.Where(tp => !tp.IsNormalDay &&
                                                                    tp.StartDayApply <= request.DayWantToPlay &&
                                                                    tp.EndDayApply >= request.DayWantToPlay).ToList();

                    // Áp dụng các TimePeriod đặc biệt
                    foreach (var item in specialDayPeriods)
                    {
                        totalPrice += CalculatePriceForTimePeriod(item, ref currentStart, request, courtSub, listCourtSubInReponse);
                    }

                    // Nếu còn thời gian chơi mà chưa được xử lý bởi các TimePeriod đặc biệt
                    if (currentStart < request.EndTimeWantToPlay)
                    {
                        // Kiểm tra các TimePeriods có IsNormalDay = true
                        var weekday = (int)request.DayWantToPlay.DayOfWeek;
                        var normalDayPeriods = timePeriods.Where(tp => tp.IsNormalDay && tp.ListDayByString.Split(',').Contains(weekday.ToString())).ToList();

                        foreach (var item in normalDayPeriods)
                        {
                            totalPrice += CalculatePriceForTimePeriod(item, ref currentStart, request, courtSub, listCourtSubInReponse);
                        }

                        // Nếu vẫn còn thời gian chơi chưa được xử lý
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
                        }
                    }

                    response.ListCourtByTimePeriod = listCourtSubInReponse;
                    response.TotalPrice = Math.Round(totalPrice, 2);
                    return response;
                }
                finally
                {
                    redisLock.ReleaseLock();
                }
            }
            else
            {
                throw new BadRequestException("Sân và khung giờ bạn muốn đặt đã được khóa bởi người khác. Vui lòng thử lại sau.");
            }
        }
    }

    private decimal CalculatePriceForTimePeriod(
        BeatSportsAPI.Domain.Entities.CourtEntity.TimePeriod.TimePeriod item,
        ref TimeSpan currentStart,
        GetBookingDetailReadyForFinishBookingQuery request,
        CourtSubdivision courtSub,
        List<CourtDetailInBookingDetailReadyForFinishBookingReponse> listCourtSubInReponse)
    {
        decimal totalPrice = 0;

        TimeSpan periodStart = item.StartTime;
        TimeSpan periodEnd = item.EndTime;

        if (currentStart < periodStart && periodStart < request.EndTimeWantToPlay)
        {
            var newCourtSubResponse = new CourtDetailInBookingDetailReadyForFinishBookingReponse()
            {
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

        if (currentStart < periodEnd && periodEnd < request.EndTimeWantToPlay)
        {
            var newCourtSubResponse = new CourtDetailInBookingDetailReadyForFinishBookingReponse()
            {
                TimePeriodId = item.Id.ToString(),
                PriceInTimePeriod = courtSub.BasePrice + item.PriceAdjustment ?? 0,
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

            totalPrice += courtSub.BasePrice * Convert.ToDecimal(timePlayInThisPeriod.TotalHours) + item.PriceAdjustment ?? 0;
            currentStart = periodEnd;
        }
        else if (currentStart < periodEnd && request.EndTimeWantToPlay < periodEnd)
        {
            var newCourtSubResponse = new CourtDetailInBookingDetailReadyForFinishBookingReponse()
            {
                TimePeriodId = item.Id.ToString(),
                PriceInTimePeriod = courtSub.BasePrice + item.PriceAdjustment ?? 0,
                TimePeriodDescription = "Khung giờ từ "
                + currentStart.ToString(@"hh\:mm")
                + " đến "
                + request.EndTimeWantToPlay.ToString(@"hh\:mm")
            };
            var timePlayInThisPeriod = request.EndTimeWantToPlay - currentStart;
            listCourtSubInReponse.Add(newCourtSubResponse);

            totalPrice += courtSub.BasePrice * Convert.ToDecimal(timePlayInThisPeriod.TotalHours) + item.PriceAdjustment ?? 0;
            currentStart = periodEnd;
        }

        return totalPrice;
    }
}
