using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetAll.GetAllCourtWithCourtSubPending;
public class GetAllCourtWithCourtSubPendingCommand : IRequest<CourtResponseV8>
{
    public Guid CourtId { get; set; }
}