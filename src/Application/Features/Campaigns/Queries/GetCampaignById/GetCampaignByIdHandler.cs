using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Courts.Queries.GetById;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Campaigns.Queries.GetCampaignById;
public class GetCampaignByIdHandler : IRequestHandler<GetCampaignByIdCommand, CampaignResponse>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetCampaignByIdHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public Task<CampaignResponse> Handle(GetCampaignByIdCommand request, CancellationToken cancellationToken)
    {
        var campaign = _dbContext.Campaigns
            .Where(x => x.Id == request.CampaignId && !x.IsDelete)
            .Include(x => x.Court)
            .ProjectTo<CampaignResponse>(_mapper.ConfigurationProvider).SingleOrDefault();
        if (campaign == null)
        {
            throw new NotFoundException($"Do not find campaign with campaign ID: {request.CampaignId}");
        }
        return Task.FromResult(campaign);
    }
}
