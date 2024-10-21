using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Commands.CreateCourtSubdivision;
public class CreateListCourtSubdivisionCommand : IRequest<BeatSportsResponse>
{
    public List<CreateCourtSubdivisionCommand> CreateListCourtSubCommands { get; set; }
}
