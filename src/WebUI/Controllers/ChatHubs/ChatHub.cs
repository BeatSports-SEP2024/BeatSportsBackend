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
        await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined!");
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
    public async Task JoinGroup(Guid customerId, string group)
    {
        var customer = _dbContext.Customers
                    .Where(x => x.Id == customerId)
                    .Include(x => x.Account)
                    .FirstOrDefault();

        var cusName = customer.Account.FirstName + " " + customer.Account.LastName;

        await Groups.AddToGroupAsync(Context.ConnectionId, group);
        await Clients.Group(group).SendAsync("ReceiveMessage", $"{cusName} joined {group}");
    }

    //gui tin nhan group private
    public async Task SendMessageToGroup(string group, Guid customerId, string message)
    {
        var customer = _dbContext.Customers
                    .Where(x => x.Id == customerId)
                    .Include(x => x.Account)
                    .FirstOrDefault();

        var cusName = customer.Account.FirstName + " " + customer.Account.LastName;

        await Clients.Group(group).SendAsync("ReceiveMessage", $"{cusName}: {message}");
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
