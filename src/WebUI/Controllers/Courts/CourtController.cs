using Azure.Core;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using BeatSportsAPI.Application.Features.Courts.Commands.CreateCourt;
using BeatSportsAPI.Application.Features.Courts.Commands.DeleteCourt;
using BeatSportsAPI.Application.Features.Courts.Commands.UpdateCourt;
using BeatSportsAPI.Application.Features.Courts.Queries.GetAll;
using BeatSportsAPI.Application.Features.Courts.Queries.GetAllCourtsByOwnerId;
using BeatSportsAPI.Application.Features.Courts.Queries.GetById;
using BeatSportsAPI.Application.Features.Courts.Queries.GetListCourtsNearBy;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.MapBox;

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
    //public async Task<PaginatedList<CourtResponse>> GetAll([FromQuery] GetAllCourtCommand request)
    //{
    //    return await _mediator.Send(request);
    //}
    //[HttpGet]
    //[Route("details")]
    //public async Task<PaginatedList<CourtWithDetailResponse>> GetAllCourtWithDetails([FromQuery] GetAllCourtWithDetailCommand request)
    //{
    //    return await _mediator.Send(request);
    //}
    [HttpGet]
    [Route("get-by-court-id")]
    public async Task<CourtResponseV3> GetByCourtId([FromQuery] GetCourtByIdCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("all")]
    public async Task<PaginatedList<CourtResponseV2>> GetAllCourt([FromQuery] GetAllCourtCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("get-by-owner-id")]
    public async Task<PaginatedList<CourtResponse>> GetByOwnerId([FromQuery] GetAllCourtsByOwnerIdCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("get-list-court-nearby")]
    public async Task<List<CourtResponseV3>> GetCourtNearBya([FromQuery] GetListCourtsNearByCommand request)
    {
        return await _mediator.Send(request);

    }
}
