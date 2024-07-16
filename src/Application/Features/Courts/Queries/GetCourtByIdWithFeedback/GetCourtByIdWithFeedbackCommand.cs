using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetCourtByIdWithFeedback;
public class GetCourtByIdWithFeedbackCommand : IRequest<CourtResponseV5>
{
    public Guid CourtId { get; set; }
}