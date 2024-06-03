﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Owners.Commands.DeleteOwner;
public class DeleteOwnerCommand : IRequest<BeatSportsResponse>
{
    public Guid OwnerId { get; set; }
}
