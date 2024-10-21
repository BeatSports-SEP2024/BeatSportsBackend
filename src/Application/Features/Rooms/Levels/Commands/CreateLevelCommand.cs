using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.Levels.Commands;
public class CreateLevelCommand : IRequest<BeatSportsResponse>
{
    public string? LevelName { get; set; }
    public string? LevelDescription { get; set; }
}