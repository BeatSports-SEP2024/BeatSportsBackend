﻿using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Bookings.Commands.CancelBooking;
using BeatSportsAPI.Application.Features.Bookings.Commands.CreateBooking;
using BeatSportsAPI.Application.Features.Bookings.Commands.DeleteBooking;
using BeatSportsAPI.Application.Features.Bookings.Commands.UpdateBooking;
using BeatSportsAPI.Application.Features.Bookings.Queries;
using BeatSportsAPI.Application.Features.Bookings.Queries.GetAllBookingHistoryByCustomerId;
using BeatSportsAPI.Application.Features.Bookings.Queries.GetAllBookingsByCustomerId;
using BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingDashboard;
using BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingDetailByCustomer;
using BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingDetailReadyForFinishBooking;
using BeatSportsAPI.Application.Features.Bookings.Queries.GetDetailBookingHistoryByCustomerId;
using BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingFinishForInvoice;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Features.Bookings.Queries.GetVenueBarchartByRangeDate;
using BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingByCourtId;
using BeatSportsAPI.Application.Features.Bookings.Queries.GetIncomeByBookingByCourtId;
using BeatSportsAPI.Application.Features.Bookings.Commands.CheckInBooking;
using BeatSportsAPI.Application.Common.Middlewares;
using BeatSportsAPI.Domain.Enums;

namespace WebAPI.Controllers.Bookings;

public class BookingController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public BookingController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet]
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<PaginatedList<BookingResponse>> GetAll([FromQuery] GetAllBookingCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("dashboard")]
    [CustomAuthorize(RoleEnums.Admin, RoleEnums.Owner)]
    public async Task<BookingDashboardResult> DashboardResult([FromQuery] GetBookingDashboardCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpPost]
    [Route("check-in")]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<BeatSportsResponse> CheckInBooking([FromBody] CheckInBookingCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpPost]
    [CustomAuthorize(RoleEnums.Customer)]
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
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<PaginatedList<BookingByCustomerId>> GetByCourtId([FromQuery] GetAllBookingsByCustomerIdCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("get-booking-detail-before-finish")]
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<BookingDetailReadyForFinishBookingResponse> GetBookingDetailReadyForFinishBookingResponse([FromQuery] GetBookingDetailReadyForFinishBookingQuery request)
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
    [HttpGet]
    [Route("get-history-by-customer-id")]
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<List<BookingHistoryByCustomerId>> GetHistoryByCustomerId([FromQuery] GetBookingHistoryByCusIdCommand request)
    {
        return await _mediator.Send(request);
    }

    [HttpGet]
    [Route("get-detail-history-by-customer-id")]
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<BookingHistoryDetailByCustomerId> GetDetailHistoryByCustomerId([FromQuery] GetDetailBookingHistoryByCusIdCommand request)
    {
        return await _mediator.Send(request);
    }

    [HttpGet]
    [Route("get-detail-history-by-booking-id")]
    [CustomAuthorize(RoleEnums.Admin, RoleEnums.Owner)]
    //[CustomAuthorize(RoleEnums.Owner)]
    public async Task<BookingHistoryDetailByCustomerId> GetDetailHistoryByBookingId([FromQuery] GetDetailBookingHistoryByBookingIdCommand request)
    {
        return await _mediator.Send(request);
    }

    [HttpGet]
    [Route("get-detail-history-by-booking-id-and-owner-id")]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<BookingHistoryDetailByCustomerId> GetDetailHistoryByBookingIdAndOwnerId([FromQuery] GetDetailBookingHistoryByBookingIdAndOwnerIdCommand request)
    {
        return await _mediator.Send(request);
    }

    [HttpPut]
    [Route("cancel-booking-process")]
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<BeatSportsResponse> CancelBookingProcess([FromBody] CancelBookingProcessCommand request)
    {
        return await _mediator.Send(request);
    }

    [HttpPut]
    [Route("cancel-booking-approve")]
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<BeatSportsResponse> CancelBookingApprove([FromBody] CancelBookingApproveCommand request)
    {
        return await _mediator.Send(request);
    }

    [HttpGet]
    [Route("invoice")]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<List<BookingFinishForInvoiceResponse>> GetInvoice([FromQuery] GetBookingFinishForInvoiceQuery request)
    {
        if (!ModelState.IsValid)
        {
            throw new BadRequestException("An error is occured");
        }
        return await _mediator.Send(request);
    }

    [HttpGet]
    [Route("venue-bar-chart")]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<List<VenueBarchartResponse>> GetVenueBarchart([FromQuery] GetVenueBarchartByRangeDateCommand request)
    {
        if (!ModelState.IsValid)
        {
            throw new BadRequestException("An error is occured");
        }
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("booking-by-court-id")]
    [CustomAuthorize(RoleEnums.Admin, RoleEnums.Owner)]
    //[CustomAuthorize(RoleEnums.Owner)]
    public async Task<PaginatedList<GetBookingByCourtIdResponse>> GetAllBookingByCourtId([FromQuery] GetBookingByCourtIdCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("income-by-court-id")]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<List<IncomeByBookingResponse>> GetIncomeByCourtId([FromQuery] GetIncomeByBookingByCourtIdQuery request)
    {
        return await _mediator.Send(request);
    }
}