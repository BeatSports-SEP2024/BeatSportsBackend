using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSportCategory.Commands;
public class DeleteCourtSportCategoryCommand : IRequest<BeatSportsResponse>
{
    [Required]
    public Guid CourtId { get; set; }
    [Required]
    public Guid SportCategoriesId { get; set; }
}