using Microsoft.AspNetCore.SignalR;

namespace BeatSportsAPI.Application.Features.Hubs;
public class RoomRequestHub : Hub
{
    public async Task UpdateRoom(string action, string customerId)
    {
        await Clients.Group(Context.ConnectionId).SendAsync("UpdateRoom", action, customerId);
    }

    public async Task JoinRoomGroup(string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
    }

    public async Task LeaveRoomGroup(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
    }

    public async Task UpdateMemberStatus(string roomRequestId, string customerId, string status)
    {
        await Clients.Group(roomRequestId).SendAsync("UpdateMemberStatus", customerId, status);
    }
}
