using BeatSportsAPI.Application.Common.Middlewares;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Accounts.Queries;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetAllCourtSubdivisionOfCourt;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisionSetting.Queries;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.CourtSubSettings;

public class CourtSubdivisionSettingController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public CourtSubdivisionSettingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<List<CourtSubSettingResponse>> GetAll([FromQuery] GetSettingBySportCategoryCommand request)
    {
        return await _mediator.Send(request);
    }
}