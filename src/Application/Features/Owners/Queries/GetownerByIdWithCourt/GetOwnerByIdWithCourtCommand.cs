using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Owners.Queries.GetownerByIdWithCourt;
public class GetOwnerByIdWithCourtCommand : IRequest<OwnerResponseV2>
{
    public Guid OwnerId { get; set; }
}