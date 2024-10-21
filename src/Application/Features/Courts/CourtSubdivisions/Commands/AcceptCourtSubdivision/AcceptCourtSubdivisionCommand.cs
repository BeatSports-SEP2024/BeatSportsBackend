using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Commands.AcceptCourtSubdivision;
public class AcceptCourtSubdivisionCommand : IRequest<BeatSportsResponse>
{
    [Required]
    public Guid CourtId { get; set; }
    [EnumDataType(typeof(StatusEnums))]
    public StatusEnums Status { get; set; }
    public string? ReasonOfReject { get; set; }
}