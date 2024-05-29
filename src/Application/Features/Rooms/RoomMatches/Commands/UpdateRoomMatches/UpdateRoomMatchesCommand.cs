using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMatches.Commands.UpdateRoomMatches;
public class UpdateRoomMatchesCommand : IRequest<BeatSportsResponse>
{
    public Guid RoomMatchId { get; set; }
    public Guid CourtId { get; set; }
    public Guid LevelId { get; set; }
    public TimeSpan StartTimeRoom { get; set; }
    public TimeSpan EndTimeRoom { get; set; }
    public int MaximumMember { get; set; }
    public string? RuleRoom { get; set; }
    public string? Note { get; set; }
}
