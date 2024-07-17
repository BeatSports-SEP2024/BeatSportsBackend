using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Commands.CreateCourtSubdivision;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Commands.DeleteCourtSubdivision;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Commands.LockCourtSubdivision;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Commands.UpdateCourtSubdivision;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetAllCourtSubdivisionOfCourt;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetCourtSubdivisionById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.CourtSubdivisions;

public class CourtSubdivisionController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public CourtSubdivisionController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpPost]
    public async Task<BeatSportsResponse> Create(CreateListCourtSubdivisionCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpPut]
    public async Task<BeatSportsResponse> Update(UpdateCourtSubdivisionCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpDelete]
    public async Task<BeatSportsResponse> Delete(DeleteCourtSubdivisionCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpPost]
    [Route("lock-court-subdivision")]
    public async Task<BeatSportsResponse> Lock(LockCourtSubdivisionCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    public async Task<PaginatedList<CourtSubdivisionResponseV3>> GetAll([FromQuery]GetAllCourtSubdivisionOfCourtQuery request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("get-by-id")]
    public async Task<CourtSubdivisionResponse?> GetById([FromQuery] GetCourtSubdivisionByIdQuery request)
    {
        return await _mediator.Send(request);
    }
}
