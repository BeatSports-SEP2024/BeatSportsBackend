using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Courts.Queries.GetAll;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;

namespace BeatSportsAPI.Application.Features.Campaigns.Queries.GetAllCampaigns;
public class GetAllCampaignHandler : IRequestHandler<GetAllCampaignsCommand, PaginatedList<CampaignResponse>>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetAllCampaignHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    public Task<PaginatedList<CampaignResponse>> Handle(GetAllCampaignsCommand request, CancellationToken cancellationToken)
    {
        if (request.PageIndex <= 0 || request.PageSize <= 0)
        {
            throw new BadRequestException("Page index and page size cannot less than 0");
        }

        IQueryable<Campaign> query = _dbContext.Campaigns
            .Where(x => !x.IsDelete)
            .OrderByDescending(b => b.Created);

        var list = query.Select(c => new CampaignResponse
        {
            CampaignId = c.Id,
            CourtId = c.CourtId,
            CampaignName = c.CampaignName,
            Description = c.Description,
            PercentDiscount = c.PercentDiscount,
            StartDateApplying = c.StartDateApplying,
            EndDateApplying = c.EndDateApplying,
            Status = c.Status,
            QuantityOfCampaign = c.QuantityOfCampaign,
            CampaignImageUrl = c.CampaignImageURL
        })
        .PaginatedListAsync(request.PageIndex, request.PageSize); 

        return list;
    }
}
