using BeatSportsAPI.Application.Common.Middlewares;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Rooms.RatingRooms.Queries.GetAllRating;
using BeatSportsAPI.Application.Features.TransactionThridparty.Queries.GetAllTransactionForAdmin;
using BeatSportsAPI.Application.Features.Wallets.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.RatingRooms;
[ApiController]
[Route("api/v1/rating")]
public class RatingRoomsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RatingRoomsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("all")]
    [CustomAuthorize]
    public async Task<List<RatingRoomsResponse>> GetAllRating()
    {
        var response = await _mediator.Send(new GetAllRatingRoomsCommand());
        return response;
    }
}
