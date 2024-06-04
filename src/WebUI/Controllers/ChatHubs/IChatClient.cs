namespace WebAPI.Controllers.ChatHubs;

public interface IChatClient
{
    Task ReceiveMessage(string message);
}
