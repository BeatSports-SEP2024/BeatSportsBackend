using Microsoft.AspNetCore.SignalR;

namespace BeatSportsAPI.Application.Features.Hubs;
public class TimePeriodHub : Hub
{
    public async Task SendTimePeriodUpdate()
    {
        await Clients.All.SendAsync("UpdateTimePeriods");
        await Clients.All.SendAsync("DeleteTimePeriods");
    }
}
