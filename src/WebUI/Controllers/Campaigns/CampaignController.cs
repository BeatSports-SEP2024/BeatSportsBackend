using BeatSportsAPI.Application.Common.Middlewares;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Campaigns.Commands.CreateCampaign;
using BeatSportsAPI.Application.Features.Campaigns.Commands.DeleteCampaign;
using BeatSportsAPI.Application.Features.Campaigns.Commands.UpdateCampaign;
using BeatSportsAPI.Application.Features.Campaigns.Commands.UpdateStatusOfCampaign;
using BeatSportsAPI.Application.Features.Campaigns.Queries.GetAllCampaigns;
using BeatSportsAPI.Application.Features.Campaigns.Queries.GetAllCampaignWithPending;
using BeatSportsAPI.Application.Features.Campaigns.Queries.GetCampaignByCourtSubdivisionAndTotalMoney;
using BeatSportsAPI.Application.Features.Campaigns.Queries.GetCampaignById;
using BeatSportsAPI.Application.Features.Campaigns.Queries.GetCampaignFilter;
using BeatSportsAPI.Application.Features.Campaigns.Queries.GetCampaignListByFilter;
using BeatSportsAPI.Application.Features.Courts.Commands.CreateCourt;
using BeatSportsAPI.Application.Features.Courts.Commands.DeleteCourt;
using BeatSportsAPI.Application.Features.Courts.Commands.UpdateCourt;
using BeatSportsAPI.Application.Features.Courts.Queries.GetAll;
using BeatSportsAPI.Application.Features.Courts.Queries.GetById;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers.Campaigns;

public class CampaignController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public CampaignController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<BeatSportsResponse> Create(CreateCampaignCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpDelete]
    [CustomAuthorize(RoleEnums.Owner)]
    //[CustomAuthorize(RoleEnums.Admin, RoleEnums.Owner)]
    public async Task<BeatSportsResponse> Delete(DeleteCampaignCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpPut]
    [CustomAuthorize(RoleEnums.Owner)]
    //[CustomAuthorize(RoleEnums.Admin, RoleEnums.Owner)]
    public async Task<BeatSportsResponse> Update(UpdateCampaignCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("all")]
    //[CustomAuthorize(RoleEnums.Admin)]
    public async Task<PaginatedList<CampaignResponseV2>> GetAll([FromQuery] GetAllCampaignsCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("get-by-campaign-id")]
    [CustomAuthorize(RoleEnums.Owner)]
    //[CustomAuthorize(RoleEnums.Owner, RoleEnums.Admin)]
    public async Task<CampaignResponseV3> GetByCampaignId([FromQuery] GetCampaignByIdCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("list-pending")]
    //[CustomAuthorize(RoleEnums.Admin)]
    public async Task<PaginatedList<CampaignResponseV2>> GetCampaignListPending([FromQuery] GetAllCampaignWithPendingCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("list-of-3")]
    [SwaggerOperation("Get top 3 of each campaign with filter: Danh sach ma khuyen mai yeu cau, ma khuyen mai lich su")]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<CampaignResult> GetCampaignListFilterTop3([FromQuery] GetCampaignFilterCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("list-campaign-filter")]
    [SwaggerOperation("Get all of each campaign with filter: Danh sach ma khuyen mai yeu cau, ma khuyen mai lich su")]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<PaginatedList<CampaignResponseV5>> GetListOfCampaignInFilter([FromQuery] GetCampaignListByFilterCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpPut]
    [Route("accept-campaign")]
    //[CustomAuthorize(RoleEnums.Admin)]
    public async Task<BeatSportsResponse> UpdateStatus(UpdateStatusOfCampaignCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("list-campaign-by-customer")]
    [SwaggerOperation("Được get lên bởi User by Court sub Id và Total Money")]
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<PaginatedList<CampaignResponseV7>> GetListOfCampaignByCustomer([FromQuery] GetCampaignByCourtSubdivisionAndTotalMoneyQuery request)
    {
        return await _mediator.Send(request);
    }
}
