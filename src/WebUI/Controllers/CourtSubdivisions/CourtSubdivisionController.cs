using BeatSportsAPI.Application.Common.Middlewares;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Campaigns.Commands.UpdateStatusOfCampaign;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Commands.AcceptCourtSubdivision;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Commands.CreateCourtSubdivision;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Commands.DeleteCourtSubdivision;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Commands.LockCourtSubdivision;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Commands.UpdateCourtSubdivision;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetAllCourtSubAvailableByTime_V1;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetAllCourtSubAvailableByTime_V2;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetAllCourtSubdivisionOfCourt;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetAllCourtSubdivisionPending;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetCourtSubdivisionAndTimeByCourtIdAndDate;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetCourtSubdivisionById;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisionSetting.Queries.GetCourtSubSettingByCourtIdAndSportCategoryId;
using BeatSportsAPI.Domain.Enums;
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
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<BeatSportsResponse> Create(CreateCourtSubdivisionCommand request)
    {
        return await _mediator.Send(request);
    }

    [HttpGet]
    [Route("get-court-sub-by-court-and-sport")]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<List<CourtSubSettingByCourtIdAndSportCategoryIdResponse>> GetCourtSubSettingByCourtIdAndSportCategoryIdQueryHandler([FromQuery]GetCourtSubSettingByCourtIdAndSportCategoryIdQuery request)
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
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<BeatSportsResponse> Lock(LockCourtSubdivisionCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<PaginatedList<CourtSubdivisionResponseV3>> GetAll([FromQuery]GetAllCourtSubdivisionOfCourtQuery request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("get-by-id")]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<CourtSubdivisionV5?> GetById([FromQuery] GetCourtSubdivisionByIdQuery request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("pending-subcourt")]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<PaginatedList<CourtSubdivisionResponseV3>> GetAllCourtSubPending([FromQuery] GetAllCourtSubdivisionPendingCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("court-and-court-sub-and-time-checking")]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<CourtSubdivisionAndTime> GetCourtAndCourtSubdivisionAndTimeChecking([FromQuery] GetCourtSubdivisionAndTimeByCourtIdAndDateQuery request)
        {
        return await _mediator.Send(request);
    }
    [HttpPut]
    [Route("accept-courtsub")]
    //[CustomAuthorize(RoleEnums.Admin)]
    public async Task<BeatSportsResponse> UpdateStatus(AcceptCourtSubdivisionCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("court-sub-checking-v1")]
    public async Task<CourtSubCheckedResponse> GetCourtSubAvailableV1([FromQuery] GetAllCourtSubAvailableByTimeV1Command request)
    {
        return await _mediator.Send(request);
    }

    [HttpGet]
    [Route("court-sub-checking-v2")]
    public async Task<CourtSubCheckedResponseV2> GetCourtSubAvailableV2([FromQuery] GetAllCourtSubAvailableByTimeV2Command request)
    {
        return await _mediator.Send(request);
    }
}