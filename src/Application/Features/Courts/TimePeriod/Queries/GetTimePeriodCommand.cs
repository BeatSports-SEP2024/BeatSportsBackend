using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.TimePeriod.Queries;
public class GetTimePeriodCommand : IRequest<List<TimePeriodResponse>>
{
    public Guid CourtId { get; set; }
    public Guid SportCategoryId { get; set; }   
}
