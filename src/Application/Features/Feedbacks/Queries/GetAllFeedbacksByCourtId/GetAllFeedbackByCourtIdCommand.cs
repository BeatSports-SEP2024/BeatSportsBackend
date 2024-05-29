using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Feedbacks.Queries.GetAllFeedbacksByCourtId;
public class GetAllFeedbackByCourtIdCommand : IRequest<PaginatedList<FeedbackResponse>>
{
    public Guid CourtId { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}
