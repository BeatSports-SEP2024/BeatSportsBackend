using BeatSportsAPI.Application.Features.Rooms.RoomRequests.Commands.CreateRoomRequests;
using BeatSportsAPI.Application.Features.Rooms.RoomRequests.Commands.UpdateRoomRequests;
using BeatSportsAPI.Application.Models.Authentication;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers.RoomRequests;

public class RoomRequestsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public RoomRequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("room-request")]
    [SwaggerOperation("Send Room Requests to Room Master")]
    public async Task<IActionResult> SendRoomRequest([FromBody] CreateRoomRequestCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }

    [HttpPost]
    [Route("approve-request")]
    [SwaggerOperation("Room Master Apporve Request to join Matching")]
    public async Task<IActionResult> ApproveRoomRequest([FromQuery] ApporveRoomRequestCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
}