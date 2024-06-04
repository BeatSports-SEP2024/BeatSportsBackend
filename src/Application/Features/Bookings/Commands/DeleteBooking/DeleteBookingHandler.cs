using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Bookings.Commands.DeleteBooking;
public class DeleteBookingHandler : IRequestHandler<DeleteBookingCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public DeleteBookingHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<BeatSportsResponse> Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
    {
        var isValidBooking = _beatSportsDbContext.Bookings
            .Where(b => b.Id == request.BookingId && !b.IsDelete)
            .FirstOrDefault();

        if(isValidBooking == null) 
        {
            throw new NotFoundException($"{request.BookingId}");
        }
        //Apply SoftDelete
        isValidBooking.IsDelete = true;
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);

        return new BeatSportsResponse{
            Message = "Delete Successfully"
        };
    }
}
