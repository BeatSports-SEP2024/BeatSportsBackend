using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Models.Authentication;
using MediatR;

namespace BeatSportsAPI.Application.Features.Authentication.Queries;
public class RefreshTokenCommandHandler : IRequest<BeatSportsResponse> {
    private readonly IIdentityService _identityService;

    public RefreshTokenCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<BeatSportsResponse> Handle(CancellationToken cancellationToken)
    {
        
        return new BeatSportsResponse
        {
            Message = "Create new user successfully"
        };
    }
}

