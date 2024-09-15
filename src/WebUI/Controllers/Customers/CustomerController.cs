using BeatSportsAPI.Application.Common.Middlewares;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetAllCourtSubdivisionOfCourt;
using BeatSportsAPI.Application.Features.Customers.Commands.DeleteCustomer;
using BeatSportsAPI.Application.Features.Customers.Commands.UpdateCustomer;
using BeatSportsAPI.Application.Features.Customers.Queries;
using BeatSportsAPI.Application.Features.Owners.Commands.DeleteOwner;
using BeatSportsAPI.Application.Features.Owners.Commands.UpdateOwner;
using BeatSportsAPI.Application.Features.Sports.Queries;
using BeatSportsAPI.Domain.Enums;
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
    [HttpDelete]
    public async Task<BeatSportsResponse> Delete(DeleteCustomerCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpPut]
    public async Task<BeatSportsResponse> Update(UpdateCustomerCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [SwaggerOperation("Get list of customers")]
    public async Task<PaginatedList<CustomerResponse>> GetAll([FromQuery] GetAllCustomersCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet("id")]
    [SwaggerOperation("Get customer by Id")]
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<IActionResult> GetCustomerById([FromQuery] GetCustomerByIdCommand request)
    {
        var response = await _mediator.Send(request);

        return Ok(response);
    }
}