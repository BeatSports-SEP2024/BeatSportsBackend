using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities;

namespace BeatSportsAPI.Application.Common.Response;
public class OwnerResponse : IMapFrom<Owner>
{
    public Guid AccountId { get; set; }
    public Guid OwnerId { get; set; }
}