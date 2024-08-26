using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Middlewares;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Feedbacks.Commands.UpdateFeedback;
using BeatSportsAPI.Application.Features.Feedbacks.Queries.GetAllFeedbacksByCourtId;
using BeatSportsAPI.Application.Features.Notifications.Commands.Update;
using BeatSportsAPI.Application.Features.Notifications.Queries.GetNotificationsByAccount;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers.PushNotification;

[Route("api/v1/notification")]
[ApiController]
public class PushNotificationController : ControllerBase
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMediator _mediator;

    public PushNotificationController(IBeatSportsDbContext beatSportsDbContext, IMediator mediator)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mediator = mediator;
    }

    [HttpGet]
    [Route("get-by-account")]
    [CustomAuthorize(RoleEnums.Customer, RoleEnums.Owner)]
    public async Task<List<NotificationResponse>> GetNotificationByAccount([FromQuery] GetNotificationByAccountCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpPut]
    [Route("is-readed")]
    [CustomAuthorize(RoleEnums.Customer, RoleEnums.Owner)]
    public async Task<BeatSportsResponse> Update(UpdateNotificationIsReadCommand request)
    {
        return await _mediator.Send(request);
    }

    [HttpPost]
    [Route("register-push-token")]
    [CustomAuthorize(RoleEnums.Customer)]
    public IActionResult RegisterPushToken([FromBody] RegisterPushTokenRequest request)
    {
        
        var user = _beatSportsDbContext.Accounts.Where(a => a.Id == request.UserId).SingleOrDefault(); 
        if (user != null)
        {
            user.ExpoPushToken = request.Token;
            _beatSportsDbContext.Accounts.Update(user);
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
