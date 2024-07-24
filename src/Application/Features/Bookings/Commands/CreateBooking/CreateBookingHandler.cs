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
        //tạo key trong Redis
        string lockKey = $"booking:{request.CourtSubdivisionId}:{request.StartTimePlaying}:lock";
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
                    // bắt đầu book
                    Console.WriteLine($"Booking {request.CustomerId} is being processed.");

                    //vd thời gian diễn ra booking
                    Task.Delay(5000).Wait();

                    var isValidCustomer = _beatSportsDbContext.Customers
                        .Where(c => c.Id == request.CustomerId && !c.IsDelete)
                        .SingleOrDefault();

                    var isValidCampaign = _beatSportsDbContext.Campaigns
                        .Where(campaigns => campaigns.Id == request.CampaignId)
                        .SingleOrDefault();

                    var isValidCourtSub = _beatSportsDbContext.CourtSubdivisions
                        .Where(cs => cs.Id == request.CourtSubdivisionId && !cs.IsDelete)
                        .SingleOrDefault();

                    var courtBookedList = _beatSportsDbContext.Bookings
                                        .Where(x => x.CustomerId == request.CustomerId && x.CourtSubdivisionId == request.CourtSubdivisionId)
                                        .ToList();

                    if (isValidCustomer == null)
                    {
                        throw new BadRequestException($"{request.CustomerId} is not existed");
                    }

                    if (isValidCampaign == null)
                    {
                        throw new BadRequestException($"{request.CampaignId} is not existed");
                    }

                    if (isValidCampaign.QuantityOfCampaign < 1)
                    {
                        throw new BadRequestException($"{isValidCampaign.QuantityOfCampaign} is not enough");
                    }

                    if (isValidCourtSub == null)
                    {
                        throw new BadRequestException($"{request.CourtSubdivisionId} is not existed");
                    }

                    if (courtBookedList.Count > 0)
                    {
                        var flag = 0;

                        foreach (var courtBooked in courtBookedList)
                        {

                            if (request.PlayingDate.Date == courtBooked.PlayingDate.Date)
                            {
                                if (request.StartTimePlaying <= courtBooked.StartTimePlaying && request.EndTimePlaying >= courtBooked.EndTimePlaying)
                                {
                                    flag++;
                                    break;
                                }
                                else if (((request.StartTimePlaying <= courtBooked.StartTimePlaying) && (courtBooked.StartTimePlaying < request.EndTimePlaying)))
                                {
                                    flag++;
                                    break;
                                }
                                else if (((request.StartTimePlaying < courtBooked.EndTimePlaying) && (courtBooked.EndTimePlaying <= request.EndTimePlaying)))
                                {
                                    flag++;
                                    break;
                                }
                            }
                        }

                        if (flag > 0)
                        {
                            return new BookingSuccessResponse
                            {
                                Message = "400"
                            };
                        }

                    }

                    var newBooking = new Booking
                    {
                        CustomerId = request.CustomerId,
                        CampaignId = request.CampaignId ?? Guid.Empty,
                        CourtSubdivisionId = request.CourtSubdivisionId,
                        BookingDate = DateTime.UtcNow,
                        TotalAmount = request.TotalAmount,
                        IsRoomBooking = request.IsRoomBooking,
                        IsDeposit = request.IsDeposit,
                        PlayingDate = request.PlayingDate,
                        StartTimePlaying = request.StartTimePlaying,
                        EndTimePlaying = request.EndTimePlaying,
                        BookingStatus = BookingEnums.Approved.ToString(),
                    };

                    if(newBooking != null)
                    {
                        isValidCampaign.QuantityOfCampaign -= 1;
                    }
                    await _beatSportsDbContext.Bookings.AddAsync(newBooking);
                    await _beatSportsDbContext.SaveChangesAsync(cancellationToken);

                    DateTime startTime = newBooking.PlayingDate.Date.Add(newBooking.StartTimePlaying);
                    DateTime endTime = newBooking.PlayingDate.Date.Add(newBooking.EndTimePlaying);
                    var courtSubLock = new TimeChecking
                    {
                        CourtSubdivisionId = newBooking.CourtSubdivisionId,
                        StartTime = startTime,
                        EndTime = endTime,
                        IsLock = true
                    };
                    await _beatSportsDbContext.TimeChecking.AddAsync(courtSubLock);
                    await _beatSportsDbContext.SaveChangesAsync(cancellationToken);

                    // Send notification after successful booking
                    //await _notificationService.SendNotificationAsync( 
                    //    new NotificationModels
                    //    {
                    //        UserId = request.CustomerId,
                    //        Title = "Booking thành công",
                    //        Body = "Đơn của bạn đang được xử lý"
                    //    });
                    //test console
                    Console.WriteLine($"Booking {request.CustomerId} is complete.");
                    return new BookingSuccessResponse
                    {
                        BookingId = newBooking.Id,
                        Message = "Booking Successfully"
                    };

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