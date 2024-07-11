using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Campaigns.Commands.CreateCampaign;
using BeatSportsAPI.Application.Features.Campaigns.Commands.DeleteCampaign;
using BeatSportsAPI.Application.Features.Campaigns.Commands.UpdateCampaign;
using BeatSportsAPI.Application.Features.Campaigns.Queries.GetAllCampaigns;
using BeatSportsAPI.Application.Features.Campaigns.Queries.GetAllCampaignWithPending;
using BeatSportsAPI.Application.Features.Campaigns.Queries.GetCampaignById;
using BeatSportsAPI.Application.Features.Courts.Commands.CreateCourt;
using BeatSportsAPI.Application.Features.Courts.Commands.DeleteCourt;
using BeatSportsAPI.Application.Features.Courts.Commands.UpdateCourt;
using BeatSportsAPI.Application.Features.Courts.Queries.GetAll;
using BeatSportsAPI.Application.Features.Courts.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.Campaigns;

public class CampaignController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public CampaignController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<BeatSportsResponse> Create(CreateCampaignCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpDelete]
    public async Task<BeatSportsResponse> Delete(DeleteCampaignCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpPut]
    public async Task<BeatSportsResponse> Update(UpdateCampaignCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("all")]
    public async Task<PaginatedList<CampaignResponseV2>> GetAll([FromQuery] GetAllCampaignsCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("get-by-campaign-id")]
    public async Task<CampaignResponseV3> GetByCampaignId([FromQuery] GetCampaignByIdCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("list-pending")]
    public async Task<PaginatedList<CampaignResponseV2>> GetCampaignListPending([FromQuery] GetAllCampaignWithPendingCommand request)
    {
        return await _mediator.Send(request);
    }
}
