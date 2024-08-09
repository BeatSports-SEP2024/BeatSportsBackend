using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMatches.Commands.CreateRoomMatches;
public class CreateRoomMatchesCommand : IRequest<RoomMatchResponse>
{
    public Guid LevelId { get; set; }
    public Guid BookingId { get; set; }
    public string? RoomName { get; set; }
    //public SportCategoriesEnums SportCategoriesEnums { get; set; }
    //public string? StartTimeRoom { get; set; }
    //public TimeSpan EndTimeRoom { get; set; }
    public int MaximumMember { get; set; }
    public string? RuleRoom { get; set; }
    public string? Note { get; set; }
    public bool IsPrivate { get; set; }
}