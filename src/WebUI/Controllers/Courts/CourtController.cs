﻿using Azure.Core;
using BeatSportsAPI.Application.Common;
using BeatSportsAPI.Application.Common.Middlewares;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingDashboard;
using BeatSportsAPI.Application.Features.Courts.Commands.CreateCourt;
using BeatSportsAPI.Application.Features.Courts.Commands.DeleteCourt;
using BeatSportsAPI.Application.Features.Courts.Commands.UpdateCourt;
using BeatSportsAPI.Application.Features.Courts.CourtSportCategory.Queries.GetCourtSportCategoryWIthCourtSubByCourtId;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetCourtSubAndSportTypeAndTimeCheckingByCourtId;
using BeatSportsAPI.Application.Features.Courts.Queries.GetAll;
using BeatSportsAPI.Application.Features.Courts.Queries.GetAll.GetAllCourtWithCourtSubPending;
using BeatSportsAPI.Application.Features.Courts.Queries.GetAllCourtsByOwnerId;
using BeatSportsAPI.Application.Features.Courts.Queries.GetById;
using BeatSportsAPI.Application.Features.Courts.Queries.GetCourtDashboard;
using BeatSportsAPI.Application.Features.Courts.Queries.GetCourtIdByAdmin;
using BeatSportsAPI.Application.Features.Courts.Queries.GetListCourtPending;
using BeatSportsAPI.Application.Features.Courts.Queries.GetListCourtsNearBy;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.MapBox;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers.Courts;

public class CourtController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public CourtController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("get-sport-category-by-court-id")]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<List<CourtSportCategoryWIthCourtSubResponse>> GetListCourtSportAndCourtSubByCourtId([FromQuery] GetCourtSportCategoryWIthCourtSubByCourtIdQuery request)
    {
        return await _mediator.Send(request);
    }

    [HttpPost]
    [CustomAuthorize(RoleEnums.Admin)]
    public async Task<BeatSportsResponse> Create(CreateCourtCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpPut]
    [CustomAuthorize(RoleEnums.Admin, RoleEnums.Owner)]
    public async Task<BeatSportsResponse> Update(UpdateCourtCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpDelete]
    [CustomAuthorize(RoleEnums.Admin, RoleEnums.Owner)]
    public async Task<BeatSportsResponse> Delete(DeleteCourtCommand request)
    {
        return await _mediator.Send(request);
    }
    //[HttpGet]
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
    [SwaggerOperation("Get Court Detail with List of feedback")]
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<CourtResponseV5> GetByCourtId([FromQuery] GetCourtByIdCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("all")]
    [CustomAuthorize(RoleEnums.Admin)]
    public async Task<PaginatedList<CourtResponseV2>> GetAllCourt([FromQuery] GetAllCourtCommand request)
    {
        return await _mediator.Send(request);
    }

    [HttpGet]
    [Route("get-by-court-id-by-admin-V2")]
    [CustomAuthorize(RoleEnums.Customer, RoleEnums.Owner)]
    public async Task<ResponseCourtDataInCourtSubAndCourtSettingsAndTimeChecking> GetByCourtIdByAdminTesting([FromQuery] GetCourtSubAndCourtSettingsAndTimeChecking request)
    {
        return await _mediator.Send(request);
    }

    [HttpGet]
    [Route("get-by-court-id-by-admin")]
    [CustomAuthorize(RoleEnums.Admin)]
    public async Task<CourtResponseV7> GetByCourtIdByAdmin([FromQuery] GetCourtByIdByAdminCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("get-court-pending-court-subdivision")]
    [CustomAuthorize(RoleEnums.Admin)]
    public async Task<PaginatedList<CourtResponseV6>> GetListCourtPending([FromQuery] GetListCourtPendingCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("get-by-owner-id")]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<PaginatedList<CourtResponse>> GetByOwnerId([FromQuery] GetAllCourtsByOwnerIdCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("get-list-court-nearby")]
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<List<CourtResponseV3>> GetCourtNearBya([FromQuery] GetListCourtsNearByCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("court-with-courtsub-pending")]
    [CustomAuthorize(RoleEnums.Admin)]
    public async Task<CourtResponseV8> GetAllCourtWithCourtSubPending([FromQuery] GetAllCourtWithCourtSubPendingCommand request)
    {
        return await _mediator.Send(request);
    }

    [HttpGet]
    [Route("dashboard")]
    [CustomAuthorize(RoleEnums.Admin)]
    public async Task<List<CourtDashboardResponse>> DashboardResult([FromQuery] GetCourtDashboardCommand request)
    {
        return await _mediator.Send(request);
    }
}