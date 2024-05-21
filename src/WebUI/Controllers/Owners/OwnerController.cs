using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Customers.Queries;
using BeatSportsAPI.Application.Features.Owners.Queries;
using CsvHelper.Configuration.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers.Owners;

public class OwnerController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public OwnerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [SwaggerOperation("Get list of owners")]
    public async Task<PaginatedList<OwnerResponse>> GetAll([FromQuery] GetAllOwnersCommand request)
    {
        return await _mediator.Send(request);
    }
}
