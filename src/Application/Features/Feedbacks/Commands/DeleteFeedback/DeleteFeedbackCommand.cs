using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Feedbacks.Commands.DeleteFeedback;
public class DeleteFeedbackCommand : IRequest<BeatSportsResponse>
{
    public Guid FeedbackId { get; set; }
}
