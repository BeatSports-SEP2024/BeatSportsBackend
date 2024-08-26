using BeatSportsAPI.Application.Common.Middlewares;
using BeatSportsAPI.Application.Features.Dashboards.GetDashboard;
using BeatSportsAPI.Application.Features.Transactions.Queries.GetAllWithdrawalRequestByOwner;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.Dashboards;

public class DashboardController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet]
    [Route("dashboard")]
    [CustomAuthorize(RoleEnums.Admin)]
    public async Task<IActionResult> GetDashboard([FromQuery] GetDashboardCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
}