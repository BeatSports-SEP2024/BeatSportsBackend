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
using BeatSportsAPI.Application.Features.Campaigns.Queries.GetCampaignById;
using BeatSportsAPI.Domain.Entities;
using MediatR;

namespace BeatSportsAPI.Application.Features.Feedbacks.Queries.GetFeedbackById;
public class GetFeedbackByIdHandler : IRequestHandler<GetFeedbackByIdCommand, FeedbackResponse>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetFeedbackByIdHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public Task<FeedbackResponse> Handle(GetFeedbackByIdCommand request, CancellationToken cancellationToken)
    {
        //var feedback = _dbContext.Feedbacks
        //    .Where(x => x.Id == request.FeedbackId && !x.IsDelete)
        //    .ProjectTo<FeedbackResponse>(_mapper.ConfigurationProvider).SingleOrDefault();
        //if (feedback == null)
        //{
        //    throw new NotFoundException($"Do not find feedback with feedback ID: {request.FeedbackId}");
        //}

        IQueryable<Feedback> query = _dbContext.Feedbacks
            .Where(x => x.Id == request.FeedbackId && !x.IsDelete);

        var feedback1 = query.Select(c => new FeedbackResponse
        {
            FeedbackId = c.Id,
            BookingId = c.BookingId,
            CourtId = c.CourtId,
            FeedbackStar = c.FeedbackStar,
            FeedbackAvailable = c.FeedbackAvailable,
            FeedbackStatus = c.FeedbackStatus,
            FeedbackContent = c.FeedbackContent,
        }).SingleOrDefault();

        if (feedback1 == null)
        {
            throw new NotFoundException($"Do not find feedback with feedback ID: {request.FeedbackId}");
        }
        return Task.FromResult(feedback1);
    }
}
