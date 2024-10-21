using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Campaigns.Queries.GetAllCampaigns;
using BeatSportsAPI.Domain.Entities;
using MediatR;

namespace BeatSportsAPI.Application.Features.Feedbacks.Queries.GetAllFeedbacks;
public class GetAllFeedbacksHandler : IRequestHandler<GetAllFeedbacksCommand, PaginatedList<FeedbackResponse>>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetAllFeedbacksHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public Task<PaginatedList<FeedbackResponse>> Handle(GetAllFeedbacksCommand request, CancellationToken cancellationToken)
    {
        if (request.PageIndex <= 0 || request.PageSize <= 0)
        {
            throw new BadRequestException("Page index and page size cannot less than 0");
        }

        IQueryable<Feedback> query = _dbContext.Feedbacks
            .Where(x => !x.IsDelete)
            .OrderByDescending(b => b.Created);

        var list = query.Select(c => new FeedbackResponse
        {
            FeedbackId = c.Id,
            BookingId = c.BookingId,
            CourtId = c.CourtId,
            FeedbackStar = c.FeedbackStar,
            FeedbackAvailable = c.FeedbackAvailable,
            FeedbackStatus = c.FeedbackStatus,
            FeedbackContent = c.FeedbackContent,
        })
        .PaginatedListAsync(request.PageIndex, request.PageSize);

        return list;
    }
}
