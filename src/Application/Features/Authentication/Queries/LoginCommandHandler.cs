using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Models.Authentication;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Authentication.Queries;
public class LoginCommandHandler : IRequestHandler<LoginModelRequest, LoginResponse>
{
    private readonly IMapper _mapper;
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IIdentityService _identityService;
    private readonly IMediator _mediator;

    public LoginCommandHandler(IMapper mapper, IBeatSportsDbContext beatSportsDbContext, IIdentityService identityService, IMediator mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
        _identityService = identityService;
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<LoginResponse> Handle(LoginModelRequest request, CancellationToken cancellationToken)
    {
        var loginResponse = await _identityService.AuthenticateAsync(request);
        if (loginResponse == null)
        {
            throw new BadRequestException("An error occur when process");
        }
        return new LoginResponse {
            Message = "Login Successfully",
            AccessToken = loginResponse
        };
    }
}
