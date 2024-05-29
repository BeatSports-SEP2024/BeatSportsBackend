﻿using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using BeatSportsAPI.Application.Features.Courts.Commands.CreateCourt;
using BeatSportsAPI.Application.Features.Courts.Commands.DeleteCourt;
using BeatSportsAPI.Application.Features.Courts.Commands.UpdateCourt;
using BeatSportsAPI.Application.Features.Courts.Queries.GetAll;
using BeatSportsAPI.Application.Features.Courts.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.Courts;

public class CourtController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public CourtController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<BeatSportsResponse> Create(CreateCourtCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpPut]
    public async Task<BeatSportsResponse> Update(UpdateCourtCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpDelete]
    public async Task<BeatSportsResponse> Delete(DeleteCourtCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    public async Task<PaginatedList<CourtResponse>> GetAll([FromQuery] GetAllCourtCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("details")]
    public async Task<PaginatedList<CourtWithDetailResponse>> GetAllCourtWithDetails([FromQuery] GetAllCourtWithDetailCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("get-by-court-id")]
    public async Task<CourtResponse> GetByCourtId([FromQuery] GetCourtByIdCommand request)
    {
        return await _mediator.Send(request);
    }
}
