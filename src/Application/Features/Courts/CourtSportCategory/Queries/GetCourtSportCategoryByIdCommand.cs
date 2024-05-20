using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSportCategory.Queries;
public class GetCourtSportCategoryByIdCommand : IRequest<CourtSportCategoryResponse>
{
    public Guid CourtSportCategoryId { get; set; }
}