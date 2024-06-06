using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Exceptions;
using MediatR;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Enums;
using Services.Redis;
using StackExchange.Redis;

namespace BeatSportsAPI.Application.Features.Bookings.Commands.CreateBooking;
public class CreateBookingHandler : IRequestHandler<CreateBookingCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IDatabase _database;

    public CreateBookingHandler(IBeatSportsDbContext beatSportsDbContext, IDatabase database)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _database = database;
    }

    public async Task<BeatSportsResponse> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
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
                    Task.Delay(10000).Wait();

                    var isValidCustomer = _beatSportsDbContext.Customers
                        .Where(c => c.Id == request.CustomerId && !c.IsDelete)
                        .SingleOrDefault();

                    var isValidCampaign = _beatSportsDbContext.Campaigns
                        .Where(campaigns => campaigns.Id == request.CampaignId)
                        .SingleOrDefault();

                    var isValidCourtSub = _beatSportsDbContext.CourtSubdivisions
                        .Where(cs => cs.Id == request.CourtSubdivisionId && !cs.IsDelete)
                        .SingleOrDefault();

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
                        BookingStatus = BookingEnums.Successfully.ToString(),
                    };
                    await _beatSportsDbContext.Bookings.AddAsync(newBooking);
                    await _beatSportsDbContext.SaveChangesAsync(cancellationToken);

                    //test console
                    Console.WriteLine($"Booking {request.CustomerId} is complete.");
                    return new BeatSportsResponse
                    {
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
                return new BeatSportsResponse
                {
                    Message = $"Booking in {request.CourtSubdivisionId} is already in progress by another instance."
                };
            }
        }
    }
}