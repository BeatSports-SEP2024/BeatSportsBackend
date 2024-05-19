using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.TimePeriod.Command;
public class UpdateTimePeriodCommand : IRequest<BeatSportsResponse>
{
    public Guid TimePeriodId { get; set; }
    public Guid CourtId { get; set; }
    public string Description { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal RateMultiplier { get; set; }
}
