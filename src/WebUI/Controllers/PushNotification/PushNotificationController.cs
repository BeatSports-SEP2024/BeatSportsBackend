using BeatSportsAPI.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers.PushNotification;

[Route("api/v1/notification")]
[ApiController]
public class PushNotificationController : ControllerBase
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public PushNotificationController(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    [HttpPost("register-push-token")]
    public IActionResult RegisterPushToken([FromBody] RegisterPushTokenRequest request)
    {
        
        var user = _beatSportsDbContext.Accounts.Where(a => a.Id == request.UserId).SingleOrDefault(); 
        if (user != null)
        {
            user.ExpoPushToken = request.Token;
            _beatSportsDbContext.SaveChanges();
            return Ok(new { Message = "Push token registered successfully." });
        }
        return BadRequest(new { Message = "User not found." });
    }

    public class RegisterPushTokenRequest
    {
        public string Token { get; set; }
        public Guid UserId { get; set; }
    }
}
