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
public class GetCampaignByIdHandler : IRequestHandler<GetCampaignByIdCommand, CampaignResponseV3>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetCampaignByIdHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public Task<CampaignResponseV3> Handle(GetCampaignByIdCommand request, CancellationToken cancellationToken)
    {
        var campaign = _dbContext.Campaigns
                    .Where(x => x.Id == request.CampaignId && !x.IsDelete)
                    .ToList();
        var court = _dbContext.Courts
                    .Where(x => x.Id == campaign.FirstOrDefault().CourtId)
                    .Include(x => x.CourtSubdivision)
                    .FirstOrDefault();
        var owner = _dbContext.Owners
                    .Where(x => x.Id == court.OwnerId)
                    .FirstOrDefault();
        var account = _dbContext.Accounts
                    .Where(x => x.Id == owner.AccountId)
                    .FirstOrDefault();

        if (campaign == null)
        {
            throw new NotFoundException($"Do not find campaign with campaign ID: {request.CampaignId}");
        }

        var campaignResult = campaign.Select(c => new CampaignResponseV3
        {
            Id = c.Id,
            CourtId = c.CourtId,
            CampaignName = c.CampaignName,
            DescriptionOfCampaign = c.Description,
            PercentDiscount = c.PercentDiscount,
            StartDateApplying = c.StartDateApplying,
            EndDateApplying = c.EndDateApplying,
            SportTypeApply = c.SportTypeApply,
            MinValueApply = c.MinValueApply,
            MaxValueDiscount = c.MaxValueDiscount,
            Created = c.Created,
            Status = c.Status,
            QuantityOfCampaign = c.QuantityOfCampaign,
            CampaignImageUrl = c.CampaignImageURL,
            ReasonOfReject = c.ReasonOfReject,
            
        }).FirstOrDefault();

        campaignResult.OwnerName = account.FirstName + " " + account.LastName;
        campaignResult.PhoneNumber = account.PhoneNumber;
        campaignResult.AddressOfOwner = owner.Address;
        campaignResult.AddressOfCourt = court.Address;
        campaignResult.DescriptionOfCourt = court.Description;
        campaignResult.TimeStartOfCourt = court.TimeStart;
        campaignResult.TimeEndOfCourt = court.TimeEnd;
        campaignResult.CourtName = court.CourtName;
        campaignResult.NumberOfCourtSub = court.CourtSubdivision.Count;

        return Task.FromResult(campaignResult);
    }
}
