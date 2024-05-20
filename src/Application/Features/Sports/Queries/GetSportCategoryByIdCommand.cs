using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Sports.Queries;
public class GetSportCategoryByIdCommand : IRequest<SportCategoriesResponse>
{
    public Guid SportCategoryId { get; set; }
}