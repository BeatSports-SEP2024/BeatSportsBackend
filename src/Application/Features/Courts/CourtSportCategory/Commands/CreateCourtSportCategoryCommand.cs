using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSportCategory.Commands;
public class CreateCourtSportCategoryCommand : IRequest<BeatSportsResponse>
{
    public Guid CourtSubdivionId { get; set; }
    public Guid SportCategoryId { get; set; }
}