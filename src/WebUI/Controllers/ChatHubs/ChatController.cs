using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Controllers.ChatHubs;
public class ChatController : ControllerBase
{
    private readonly IHubContext<ChatHub> _hubContext;
    public ChatController(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    [HttpGet]
    [Route("push-message-to-world-channel")]
    public IActionResult PushMessage(string message)
    {
        _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
        return Ok("Done");
    }
}
