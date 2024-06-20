﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetAllCourtsByOwnerId;
public class GetAllCourtsByOwnerIdCommand : IRequest<PaginatedList<CourtResponse>>
{
    [Required]
    public Guid OwnerId { get; set; }
    [Required]
    public int PageIndex { get; set; }
    [Required]
    public int PageSize { get; set; }
}