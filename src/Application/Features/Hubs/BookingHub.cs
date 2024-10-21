using Microsoft.AspNetCore.SignalR;

namespace BeatSportsAPI.Application.Features.Hubs;
public class BookingHub : Hub
{
    public async Task SendBookingUpdate()
    {
        await Clients.All.SendAsync("CreateBooking");
        await Clients.All.SendAsync("DeleteBooking");
        await Clients.All.SendAsync("DeleteBookingProcess");
    }
}
