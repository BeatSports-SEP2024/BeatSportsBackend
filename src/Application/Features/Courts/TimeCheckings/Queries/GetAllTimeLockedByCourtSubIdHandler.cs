using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Feedbacks.Queries.GetAllFeedbacksByCourtId;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.TimeCheckings.Queries;
public class GetAllTimeLockedByCourtSubIdHandler : IRequestHandler<GetAllTimeLockedByCourtSubIdCommand, List<TimeChecking>>
{
    private readonly IBeatSportsDbContext _dbContext;
   
    public GetAllTimeLockedByCourtSubIdHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<TimeChecking>> Handle(GetAllTimeLockedByCourtSubIdCommand request, CancellationToken cancellationToken)
    {
        
        IQueryable<TimeChecking> query = _dbContext.TimeChecking
            .Where(x => x.CourtSubdivisionId == request.CourtSubId && !x.IsDelete);

        return Task.FromResult(query.ToList());
    }
}
