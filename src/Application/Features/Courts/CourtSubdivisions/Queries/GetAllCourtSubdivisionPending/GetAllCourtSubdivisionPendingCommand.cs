﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetAllCourtSubdivisionPending;
public class GetAllCourtSubdivisionPendingCommand : IRequest<PaginatedList<CourtSubdivisionResponseV3>>
{
    public Guid CourtId { get; set; }
    [Required]
    public int PageIndex { get; set; }
    [Required]
    public int PageSize { get; set; }
}