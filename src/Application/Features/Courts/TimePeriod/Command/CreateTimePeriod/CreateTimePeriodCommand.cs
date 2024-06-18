using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.TimePeriod.Command.CreateTimePeriod;
public class CreateTimePeriodCommand : IRequest<BeatSportsResponse>
{
    [Required]
    public Guid CourtSubdivisionId { get; set; }
    //public string Description { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    //public decimal RateMultiplier { get; set; } (Available when update)
}