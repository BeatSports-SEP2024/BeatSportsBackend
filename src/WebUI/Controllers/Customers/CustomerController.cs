﻿using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetAllCourtSubdivisionOfCourt;
using BeatSportsAPI.Application.Features.Customers.Queries;
using BeatSportsAPI.Application.Features.Sports.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers.Customers;

public class CustomerController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public CustomerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [SwaggerOperation("Get list of customers")]
    public async Task<PaginatedList<CustomerResponse>> GetAll([FromQuery] GetAllCustomersCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet("id")]
    [SwaggerOperation("Get customer by Id")]
    public async Task<IActionResult> GetCustomerById([FromQuery] GetCustomerByIdCommand request)
    {
        var response = await _mediator.Send(request);

        return Ok(response);
    }
}