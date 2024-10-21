using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Feedbacks.Queries.GetFeedbackByBookingId;
public class GetFeedbackByBookingIdCommand : IRequest<FeedbackResponse>
{
    public Guid BookingId { get; set; }
}

public class GetFeedbackByBookingIdCommandHandler : IRequestHandler<GetFeedbackByBookingIdCommand, FeedbackResponse>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetFeedbackByBookingIdCommandHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public async Task<FeedbackResponse> Handle(GetFeedbackByBookingIdCommand request, CancellationToken cancellationToken)
    {
        var feedbackExist = await _dbContext.Feedbacks.Where(f => f.BookingId == request.BookingId).SingleOrDefaultAsync();
        if (feedbackExist == null)
        {
            throw new NotFoundException("Not Found"); 
        }

        var response = new FeedbackResponse
        {
            FeedbackId = feedbackExist.Id,
            BookingId = feedbackExist.BookingId,
            CourtId = feedbackExist.CourtId,
            FeedbackStar = feedbackExist.FeedbackStar,
            FeedbackAvailable = feedbackExist.FeedbackAvailable,
            FeedbackStatus = feedbackExist.FeedbackStatus,
            FeedbackContent = feedbackExist.FeedbackContent,
        };

        return response;
    }
}
