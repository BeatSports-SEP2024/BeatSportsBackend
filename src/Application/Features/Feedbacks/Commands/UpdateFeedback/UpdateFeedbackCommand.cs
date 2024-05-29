using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Feedbacks.Commands.UpdateFeedback;
public class UpdateFeedbackCommand : IRequest<BeatSportsResponse>
{
    [Required]
    public Guid FeedbackId { get; set; }
    public decimal FeedbackStar { get; set; }
    public bool FeedbackAvailable { get; set; }
    public string? FeedbackStatus { get; set; }
    public string FeedbackContent { get; set; }
}
