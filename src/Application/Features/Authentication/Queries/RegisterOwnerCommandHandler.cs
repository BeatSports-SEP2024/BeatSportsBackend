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
public class RegisterOwnerCommandHandler : IRequestHandler<RegisterOwnerModelRequest, BeatSportsResponse>
{
    private readonly IIdentityService _identityService;

    public RegisterOwnerCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    public async Task<BeatSportsResponse> Handle(RegisterOwnerModelRequest request, CancellationToken cancellationToken)
    {
        var registerResponse = await _identityService.RegisterOwnerAccountAsync(request, cancellationToken);
        if (registerResponse == null)
        {
            throw new BadRequestException("An error is occurred when process");
        }

        return new BeatSportsResponse
        {
            Message = "Create new user successfully"
        };
    }
}
