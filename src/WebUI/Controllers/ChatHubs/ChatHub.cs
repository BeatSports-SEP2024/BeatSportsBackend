using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Controllers.ChatHubs;

public sealed class ChatHub : Hub<IChatClient>
{
    //thong bao co nguoi join server
    public override async Task OnConnectedAsync()
    {
        await Clients.All.ReceiveMessage($"{Context.ConnectionId} has joined!");
    }

    //gui tin nhan kenh the gioi
    public async Task SendMessage(string message)
    {
        await Clients.All.ReceiveMessage($"{Context.ConnectionId}: {message}");
    }

    //tham gia group private
    public async Task JoinGroup(string group)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, group);
        await Clients.Group(group).ReceiveMessage($"{Context.ConnectionId} joined {group}");
    }

    //gui tin nhan group private
    public async Task SendMessageToGroup(string group, string message)
    {
        await Clients.Group(group).ReceiveMessage($"{Context.ConnectionId}: {message}");
    }
    //out group private
    public async Task OutGroup(string group)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        await Clients.Group(group).ReceiveMessage($"{Context.ConnectionId} out {group}");
    }
}
