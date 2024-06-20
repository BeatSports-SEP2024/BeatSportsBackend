using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.TimeCheckings.Queries;
public class GetAllTimeLockedByCourtSubIdCommand : IRequest<List<TimeChecking>>
{
    public Guid CourtSubId { get; set; }
}
