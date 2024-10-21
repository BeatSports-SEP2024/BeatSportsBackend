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
using BeatSportsAPI.Application.Features.Feedbacks.Queries.GetAllFeedbacks;
using BeatSportsAPI.Application.Features.Feedbacks.Queries.GetFeedbackById;
using BeatSportsAPI.Domain.Entities;
using MediatR;

namespace BeatSportsAPI.Application.Features.Feedbacks.Queries.GetAllFeedbacksByCourtId;
public class GetAllFeedbackByCourtIdHandler : IRequestHandler<GetAllFeedbackByCourtIdCommand, PaginatedList<FeedbackResponse>>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetAllFeedbackByCourtIdHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public Task<PaginatedList<FeedbackResponse>> Handle(GetAllFeedbackByCourtIdCommand request, CancellationToken cancellationToken)
    {
        if (request.PageIndex <= 0 || request.PageSize <= 0)
        {
            throw new BadRequestException("Page index and page size cannot less than 0");
        }

        IQueryable<Feedback> query = _dbContext.Feedbacks
            .Where(x => x.CourtId == request.CourtId && !x.IsDelete)
            .OrderByDescending(x => x.Created);

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
