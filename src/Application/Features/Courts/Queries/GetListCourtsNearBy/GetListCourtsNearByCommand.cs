using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using MediatR;
using Services.MapBox;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetListCourtsNearBy;
public class GetListCourtsNearByCommand : IRequest<List<CourtResponse>>
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public Guid CourtId { get; set; }

}
