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
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Application.Features.Courts.Queries.GetAll;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;

namespace BeatSportsAPI.Application.Features.Campaigns.Queries.GetAllCampaigns;
public class GetAllCampaignHandler : IRequestHandler<GetAllCampaignsCommand, PaginatedList<CampaignResponseV2>>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetAllCampaignHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    public Task<PaginatedList<CampaignResponseV2>> Handle(GetAllCampaignsCommand request, CancellationToken cancellationToken)
    {
        if (request.PageIndex <= 0 || request.PageSize <= 0)
        {
            throw new BadRequestException("Page index and page size cannot less than 0");
        }

        var query = _dbContext.Campaigns
            .Where(x => !x.IsDelete);

        if (request.StartDate.HasValue && request.EndDate.HasValue)
        {
            query = query.Where(tp => tp.StartDateApplying.Date >= request.StartDate.Value.Date
                    && tp.EndDateApplying.Date <= request.EndDate.Value.Date);
        }
        else if (request.StartDate.HasValue)
        {
            query = query.Where(tp => tp.StartDateApplying.Date >= request.StartDate.Value.Date);
        }
        else if (request.EndDate.HasValue)
        {
            query = query.Where(tp => tp.EndDateApplying.Date <= request.EndDate.Value.Date);
        }
        query = query.OrderByDescending(tp => tp.Created);
        var list = query.Select(c => new CampaignResponseV2
        {
            CampaignId = c.Id,
            CourtId = c.CourtId,
            CampaignName = c.CampaignName,
            StartDateApplying = c.StartDateApplying,
            EndDateApplying = c.EndDateApplying,
            SportTypeApply = c.SportTypeApply,
            Created = c.Created,
            Status = c.Status.ToString(),
        })
        .PaginatedListAsync(request.PageIndex, request.PageSize); 

        return list;
    }
}
