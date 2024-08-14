using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Bookings.Commands.CheckInBooking;
/// <summary>
/// Dùng để chuyển status từ Approve sang Finish  mà không cần dùng đến cron job
/// </summary>
public class CheckInBookingCommand : IRequest<BeatSportsResponse>
{
    public Guid BookingId { get; set; }
}

public class CheckInBookingCommandHandler : IRequestHandler<CheckInBookingCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public CheckInBookingCommandHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BeatSportsResponse> Handle(CheckInBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = _dbContext.Bookings.Where(x => x.Id == request.BookingId).SingleOrDefault();
        if (booking != null)
        {
            // Như đã định nghĩa trong Booking Entity, Chỉ đổi trạng thái check in, còn lại để Cronjob quét
            booking.IsCheckIn = true;
            //booking.BookingStatus = BookingEnums.Finished.ToString();
            _dbContext.Bookings.Update(booking);
            await _dbContext.SaveChangesAsync();
        }
        return new BeatSportsResponse()
        {
            Message = "Check in thành công!"
        };
    }
}