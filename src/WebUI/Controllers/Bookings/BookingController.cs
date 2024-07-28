using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Bookings.Commands.CreateBooking;
using BeatSportsAPI.Application.Features.Bookings.Commands.DeleteBooking;
using BeatSportsAPI.Application.Features.Bookings.Commands.UpdateBooking;
using BeatSportsAPI.Application.Features.Bookings.Queries;
using BeatSportsAPI.Application.Features.Bookings.Queries.GetAllBookingsByCustomerId;
using BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingDashboard;
using BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingDetailByCustomer;
using BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingDetailReadyForFinishBooking;
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
    [HttpGet]
    [Route("dashboard")]
    public async Task<BookingDashboardResult> DashboardResult([FromQuery] GetBookingDashboardCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingCommand request)
    {
        var response = await _mediator.Send(request);

        if (response.Message.Equals("400"))
        {
            response.Message = "Bạn đã book sân trong thời gian này!";
            return BadRequest(response);
        }
        return Ok(response);
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
    public async Task<PaginatedList<BookingByCustomerId>> GetByCourtId([FromQuery] GetAllBookingsByCustomerIdCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("get-booking-detail-before-finish")]
    public async Task<BookingDetailReadyForFinishBookingResponse> GetBookingDetailReadyForFinishBookingResponse([FromQuery]GetBookingDetailReadyForFinishBookingQuery request)
    {
        var response = await _mediator.Send(request);
        return response;
    }
    [HttpGet]
    [Route("booking-detail")]
    public async Task<List<BookingDetailByCustomer>> GetBookingDetailByCustomer([FromQuery] GetBookingDetailByCustomerCommand request)
    {
        var response = await _mediator.Send(request);
        return response;
    }
}
