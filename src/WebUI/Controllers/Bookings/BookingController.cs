using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Bookings.Commands.CreateBooking;
using BeatSportsAPI.Application.Features.Bookings.Commands.DeleteBooking;
using BeatSportsAPI.Application.Features.Bookings.Commands.UpdateBooking;
using BeatSportsAPI.Application.Features.Bookings.Queries;
using BeatSportsAPI.Application.Features.Bookings.Queries.GetAllBookingsByCustomerId;
using BeatSportsAPI.Application.Features.Campaigns.Commands.CreateCampaign;
using BeatSportsAPI.Application.Features.Campaigns.Commands.DeleteCampaign;
using BeatSportsAPI.Application.Features.Campaigns.Commands.UpdateCampaign;
using BeatSportsAPI.Application.Features.Campaigns.Queries.GetAllCampaigns;
using BeatSportsAPI.Application.Features.Feedbacks.Queries.GetAllFeedbacksByCourtId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.Bookings;

public class BookingController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public BookingController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet]
    public async Task<PaginatedList<BookingResponse>> GetAll([FromQuery] GetAllBookingCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpPost]
    public async Task<BeatSportsResponse> Create(CreateBookingCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpDelete]
    public async Task<BeatSportsResponse> Delete(DeleteBookingCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpPut]
    public async Task<BeatSportsResponse> Update(UpdateBookingCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("get-by-customer-id")]
    public async Task<PaginatedList<BookingResponse>> GetByCourtId([FromQuery] GetAllBookingsByCustomerIdCommand request)
    {
        return await _mediator.Send(request);
    }
}
