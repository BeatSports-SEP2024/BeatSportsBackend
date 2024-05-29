using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Campaigns.Commands.CreateCampaign;
using BeatSportsAPI.Application.Features.Campaigns.Commands.DeleteCampaign;
using BeatSportsAPI.Application.Features.Campaigns.Commands.UpdateCampaign;
using BeatSportsAPI.Application.Features.Campaigns.Queries.GetAllCampaigns;
using BeatSportsAPI.Application.Features.Campaigns.Queries.GetCampaignById;
using BeatSportsAPI.Application.Features.Feedbacks.Commands.CreateFeedback;
using BeatSportsAPI.Application.Features.Feedbacks.Commands.DeleteFeedback;
using BeatSportsAPI.Application.Features.Feedbacks.Commands.UpdateFeedback;
using BeatSportsAPI.Application.Features.Feedbacks.Queries.GetAllFeedbacks;
using BeatSportsAPI.Application.Features.Feedbacks.Queries.GetFeedbackById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.Feedbacks;

public class FeedbackController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public FeedbackController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<BeatSportsResponse> Create(CreateFeedbackCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpDelete]
    public async Task<BeatSportsResponse> Delete(DeleteFeedbackCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpPut]
    public async Task<BeatSportsResponse> Update(UpdateFeedbackCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("all")]
    public async Task<PaginatedList<FeedbackResponse>> GetAll([FromQuery] GetAllFeedbacksCommand request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [Route("get-by-feedback-id")]
    public async Task<FeedbackResponse> GetByCampaignId([FromQuery] GetFeedbackByIdCommand request)
    {
        return await _mediator.Send(request);
    }
}
