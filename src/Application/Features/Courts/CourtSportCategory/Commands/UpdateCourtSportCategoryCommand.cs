using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSportCategory.Commands;
public class UpdateCourtSportCategoryCommand : IRequest<BeatSportsResponse>
{
    public Guid CourtId { get; set; }
    public Guid SportCategoryId { get; set; }
}
