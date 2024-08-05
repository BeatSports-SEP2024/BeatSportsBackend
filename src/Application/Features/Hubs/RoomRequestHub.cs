using Microsoft.AspNetCore.SignalR;

namespace BeatSportsAPI.Application.Features.Hubs;
public class RoomRequestHub : Hub
{
    public async Task UpdateRoom(string action, string customerId, string roomId)
    {
        await Clients.Group(roomId).SendAsync("UpdateRoom", action, customerId);
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

    public async Task UpdateRoomList(/*string action, string roomId*/)
    {
        await Clients.All.SendAsync("UpdateRoomList"/*, action, roomId*/);
    }
}
