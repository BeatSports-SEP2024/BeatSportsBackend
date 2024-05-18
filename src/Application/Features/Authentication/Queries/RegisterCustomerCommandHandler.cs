using AutoMapper;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Application.Models.Authentication;
using BeatSportsAPI.Domain.Entities;
using MediatR;

namespace BeatSportsAPI.Application.Features.Authentication.Queries;
public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerModelRequest, BeatSportsResponse>
{
    private readonly IIdentityService _identityService;

    public RegisterCustomerCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<BeatSportsResponse> Handle(RegisterCustomerModelRequest request, CancellationToken cancellationToken)
    {
        var registerResponse = await _identityService.RegisterCustomerAccountAsync(request, cancellationToken);
        if(registerResponse  == null)
        {
            throw new BadRequestException("An error is occurred when process");
        }

        return new BeatSportsResponse
        {
            Message = "Create new user successfully"
        };
    }
}
