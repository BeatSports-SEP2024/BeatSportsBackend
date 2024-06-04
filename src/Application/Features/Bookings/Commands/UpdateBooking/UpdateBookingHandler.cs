using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Bookings.Commands.UpdateBooking;
public class UpdateBookingHandler : IRequestHandler<UpdateBookingCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public UpdateBookingHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<BeatSportsResponse> Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
    {
        var isValidBooking = _beatSportsDbContext.Bookings
            .Where(b => b.Id == request.BookingId && !b.IsDelete)
            .FirstOrDefault();

        if(isValidBooking == null)
        {
            throw new NotFoundException($"{request.BookingId} is not existed or delete");
        }

        isValidBooking.CustomerId = request.CustomerId;
        isValidBooking.CampaignId = request.CampaignId;
        isValidBooking.CourtSubdivisionId = request.CourtSubdivisionId;
        isValidBooking.BookingDate = request.BookingDate;
        isValidBooking.TotalAmount = request.TotalAmount;
        isValidBooking.IsRoomBooking = request.IsRoomBooking;
        isValidBooking.IsDeposit = request.IsDeposit;
        isValidBooking.PlayingDate = request.PlayingDate;
        isValidBooking.StartTimePlaying = request.StartTimePlaying;
        isValidBooking.EndTimePlaying = request.EndTimePlaying;
        isValidBooking.BookingStatus = request.BookingStatus;

        _beatSportsDbContext.Bookings.Update(isValidBooking);
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);

        return new BeatSportsResponse
        {
            Message = "Update booking successfully"
        };
    }
}
