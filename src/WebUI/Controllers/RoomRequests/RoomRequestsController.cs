using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Rooms.RoomRequests.Commands.CreateRoomRequests;
using BeatSportsAPI.Application.Features.Rooms.RoomRequests.Commands.UpdateRoomRequests;
using BeatSportsAPI.Application.Features.Rooms.RoomRequests.Queries.GetRoomRequestByCustomer;
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

        if (response.Message.Equals("400"))
        {
            response.Message = "Bạn đã ở trong một phòng khác cùng thời gian!";
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost]
    [Route("approve-request")]
    [SwaggerOperation("Room Master Apporve Request to join Matching")]
    public async Task<IActionResult> ApproveRoomRequest([FromBody] ApporveRoomRequestCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }

    [HttpPost]
    [Route("out-room-request")]
    [SwaggerOperation("Room member out room")]
    public async Task<IActionResult> MemberOutRoomRequest([FromBody] UpdateRoomRequestCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _mediator.Send(request);

        return Ok(response);
    }

    [HttpGet]
    [Route("room-request-pending")]
    [SwaggerOperation("Lấy các phòng đang chờ duyệt")]
    public async Task<List<RoomRequestResponseForCustomer>> GetPendingRoomByCusId([FromQuery] GetRoomRequestByCustomerCommand request)
    {
        return await _mediator.Send(request);
    }
}