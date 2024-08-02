using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Dashboards.GetDashboard;
public class GetDashboardCommand : IRequest<DashboardResponse>
{
    [Range(1000,9999)]
    public int Year { get; set; }
}