using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Common;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.TimePeriod.Command;
public class DeleteTimePeriodCommand : IRequest<BeatSportsResponse>
{
    public Guid TimePeriodId {  get; set; }
}
