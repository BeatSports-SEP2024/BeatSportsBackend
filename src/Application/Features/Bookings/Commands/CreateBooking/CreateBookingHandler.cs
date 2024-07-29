using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Exceptions;
using MediatR;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Enums;
using Services.Redis;
using StackExchange.Redis;
using BeatSportsAPI.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using BeatSportsAPI.Domain.Entities.Room;
using BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingDashboard;
using Newtonsoft.Json.Linq;

namespace BeatSportsAPI.Application.Features.Bookings.Commands.CreateBooking;
public class CreateBookingHandler : IRequestHandler<CreateBookingCommand, BookingSuccessResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IDatabase _database;
    private readonly INotificationService _notificationService;

    public CreateBookingHandler(IBeatSportsDbContext beatSportsDbContext, IDatabase database, INotificationService notificationService)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _database = database;
        _notificationService = notificationService;
    }

    public async Task<BookingSuccessResponse> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {

        var checkBookingInDB = await _beatSportsDbContext.Bookings.Where(x => x.Id == request.BookingId && x.BookingStatus == BookingEnums.Process.ToString() && !x.IsDelete).SingleOrDefaultAsync();
        if (checkBookingInDB == null)
        {
            throw new NotFoundException("Không tìm thấy booking như trên");
        }
        //tạo key trong Redis
        string lockKey = $"booking:{checkBookingInDB.CourtSubdivisionId}:{checkBookingInDB.StartTimePlaying}:lock";
        //tạo value tương ứng với key vừa tạo trong Redis
        string lockValue = Guid.NewGuid().ToString();
        //thời gian hết hạn
        TimeSpan expiry = TimeSpan.FromSeconds(30);
        using (var redisLock = new RedisLock(_database, lockKey, lockValue, expiry))
        {
            if (redisLock.AcquireLock())
            {
                try
                {
                    // Check lại giá tổng coi đã đúng hay chưa.
                    if (request.CampaignId != null)
                    {
                        var campaignIfExist = await _beatSportsDbContext.Campaigns.Where(x => x.Id == request.CampaignId
                        && x.QuantityOfCampaign > 0
                        && x.EndDateApplying >= checkBookingInDB.PlayingDate
                        && !x.IsDelete).SingleOrDefaultAsync();
                        if (campaignIfExist == null)
                        {
                            throw new BadRequestException("Campaign đã hết số lượng sử dụng hoặc hết hạn sử dụng");
                        }
                        // Nếu có tồn tại thì check lại giá tiền
                        // Đối với booking status process được lưu trong db thì giá đang là giá ch apply campaign
                        // Tính giá trị giảm trước
                        var value = (checkBookingInDB.TotalAmount * campaignIfExist.PercentDiscount) / 100;
                        decimal realApply = 0;
                        if (value > campaignIfExist.MaxValueDiscount)
                        {
                            realApply = campaignIfExist.MaxValueDiscount;
                        }
                        else
                        {
                            realApply = value;
                        }
                        var checkTotalMoney = checkBookingInDB.TotalAmount - realApply;
                        if (checkTotalMoney != request.TotalAmount)
                        {
                            throw new BadRequestException("Tiền không đúng với giá trị thực tế khi áp dụng mã giảm giá");
                        }
                        //Update Campaign to Booking
                        checkBookingInDB.CampaignId = request.CampaignId;   
                        checkBookingInDB.TotalPriceDiscountCampaign = realApply;
                        checkBookingInDB.BookingStatus = BookingEnums.Approved.ToString();
                        checkBookingInDB.TotalAmount = checkTotalMoney;
                        _beatSportsDbContext.Bookings.Update(checkBookingInDB);
                        await _beatSportsDbContext.SaveChangesAsync();
                        Console.WriteLine($"Booking {checkBookingInDB.CustomerId} is complete.");
                        return new BookingSuccessResponse
                        {
                            BookingId = checkBookingInDB.Id,
                            Message = "Booking Successfully"
                        };
                    }
                    else
                    {
                        checkBookingInDB.BookingStatus = BookingEnums.Approved.ToString();
                        _beatSportsDbContext.Bookings.Update(checkBookingInDB);
                        await _beatSportsDbContext.SaveChangesAsync();
                        Console.WriteLine($"Booking {checkBookingInDB.CustomerId} is complete.");
                        return new BookingSuccessResponse
                        {
                            BookingId = checkBookingInDB.Id,
                            Message = "Booking Successfully"
                        };
                    }
                }
                finally
                {
                    redisLock.ReleaseLock();
                }
            }
            else
            {
                //test console
                Console.WriteLine($"Booking in {request.CourtSubdivisionId} is already in progress by another instance.");
                return new BookingSuccessResponse
                {
                    Message = $"Booking in {request.CourtSubdivisionId} is already in progress by another instance."
                };
            }
        }
    }
}