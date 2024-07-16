using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Application.Features.Courts.Commands.CreateCourt;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Campaigns.Commands.CreateCampaign;
public class CreateCampaignHandler : IRequestHandler<CreateCampaignCommand, BeatSportsResponse>
{

    private readonly IBeatSportsDbContext _dbContext;

    public CreateCampaignHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<BeatSportsResponse> Handle(CreateCampaignCommand request, CancellationToken cancellationToken)
    {
        //check Court
        var court = _dbContext.Courts.Where(x => x.Id == request.CourtId).SingleOrDefault();
        if (court == null || court.IsDelete)
        {
            throw new BadRequestException($"Court with Court ID:{request.CourtId} does not exist or have been delele");
        }

        var endDate = request.EndDateApplying.Date
                        .AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);

        var sportTypes = String.Join(",", request.SportTypeApply.Select(e => e.GetDescriptionFromEnum()).ToArray());
        var campaign = new Campaign()
        {
            CourtId = request.CourtId,
            CampaignName = request.CampaignName,
            Description = request.Description,
            PercentDiscount = request.PercentDiscount,
            StartDateApplying = request.StartDateApplying,
            EndDateApplying = endDate,
            SportTypeApply = sportTypes,
            MinValueApply = request.MinValueApply,
            MaxValueDiscount = request.MaxValueDiscount,
            Status = StatusEnums.Pending,
            QuantityOfCampaign = request.QuantityOfCampaign,
            CampaignImageURL = request.CampaignImageUrl,
            ReasonOfReject = ""
        };
        _dbContext.Campaigns.Add(campaign);
        _dbContext.SaveChanges();
        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Create Campaign successfully!"
        });
    }
}
