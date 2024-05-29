using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Feedbacks.Commands.CreateFeedback;
public class CreateFeedbackCommand : IRequest<BeatSportsResponse>
{
    [Required]
    public Guid BookingId { get; set; }
    [Required]
    public Guid CourtId { get; set; }
    public decimal FeedbackStar { get; set; }
    public bool FeedbackAvailable { get; set; }
    public string? FeedbackStatus { get; set; }
}
