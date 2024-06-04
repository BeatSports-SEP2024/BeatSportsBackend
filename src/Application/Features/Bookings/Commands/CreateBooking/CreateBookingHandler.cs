using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Exceptions;
using MediatR;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Enums;

namespace BeatSportsAPI.Application.Features.Bookings.Commands.CreateBooking;
public class CreateBookingHandler : IRequestHandler<CreateBookingCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public CreateBookingHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<BeatSportsResponse> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
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

        if(isValidCampaign.QuantityOfCampaign < 1)
        {
            throw new BadRequestException($"{isValidCampaign.QuantityOfCampaign} is not enough");
        }

        if(isValidCourtSub == null)
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
        return new BeatSportsResponse
        {
            Message = "Booking Successfully"
        };
    }
}