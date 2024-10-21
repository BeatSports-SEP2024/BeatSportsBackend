using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers.ChatHubs;

public sealed class ChatHub : Hub
{
    private IBeatSportsDbContext _dbContext;

    public ChatHub(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    //thong bao co nguoi join server
    public override async Task OnConnectedAsync()
    {
        //await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined!");
        await base.OnConnectedAsync();
    }

    //gui tin nhan kenh the gioi
    public async Task SendMessage(Guid customerId, string message)
    {
        var customer = _dbContext.Customers
                    .Where(x => x.Id == customerId)
                    .Include(x => x.Account)
                    .FirstOrDefault();

        var cusName = customer.Account.FirstName.Trim() + " " + customer.Account.LastName.Trim();

        await Clients.All.SendAsync("ReceiveMessage", $"{cusName}: {message}", customerId.ToString());
    }

    //tham gia group private
    public async Task JoinGroup(Guid customerId, Guid roomId)
    {
        var customer = _dbContext.Customers
                    .Where(x => x.Id == customerId)
                    .Include(x => x.Account)
                    .FirstOrDefault();

        var cusName = customer.Account.FirstName.Trim() + " " + customer.Account.LastName.Trim();

        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
        //await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage",/* $"{cusName} joined {roomId}",*/ customerId.ToString());
    }

    //gui tin nhan group private
    public async Task SendMessageToGroup(Guid roomId, Guid customerId, string message)
    {
        var customer = _dbContext.Customers
                    .Where(x => x.Id == customerId)
                    .Include(x => x.Account)
                    .FirstOrDefault();

        var cusName = customer.Account.FirstName.Trim() + " " + customer.Account.LastName.Trim();

        await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", $"{cusName}: {message}", customerId.ToString());
    }
    //out group private
    public async Task OutGroup(Guid customerId, string group)
    {
        var customer = _dbContext.Customers
                    .Where(x => x.Id == customerId)
                    .Include(x => x.Account)
                    .FirstOrDefault();

        var cusName = customer.Account.FirstName + " " + customer.Account.LastName;

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        await Clients.Group(group).SendAsync("ReceiveMessage", $"{cusName} out {group}");
    }
}
