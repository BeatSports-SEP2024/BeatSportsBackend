using AutoMapper;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Authentication.Command.AuthGoogle;
public class LoginGoogleAuthCommandHandler : IRequestHandler<GoogleLoginRequest, GoogleLoginResponse>
{
    private readonly IMapper _mapper;
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IIdentityService _identityService;
    private readonly IMediator _mediator;

    public LoginGoogleAuthCommandHandler(
        IMapper mapper, 
        IBeatSportsDbContext beatSportsDbContext, 
        IIdentityService identityService, 
        IMediator mediator
        )
    {
        _mapper = mapper;
        _mediator = mediator;
        _identityService = identityService;
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<GoogleLoginResponse> Handle(GoogleLoginRequest request, CancellationToken cancellationToken)
    {
        var googleLoginResponse = await _identityService.GoogleLoginAuthAsync(request, cancellationToken);
        if (googleLoginResponse == null)
        {
            throw new BadRequestException("An error occur when process");
        }
        return new GoogleLoginResponse
        {
            AccessToken = googleLoginResponse.AccessToken,
            User = googleLoginResponse.User,
        };
    }
}
