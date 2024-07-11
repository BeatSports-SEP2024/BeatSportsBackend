using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Owners.Queries.GetOwnerId;
public class GetOwnerByIdCommand : IRequest<OwnerResponse>
{
    [Required]
    public Guid OwnerId { get; set; }
}
