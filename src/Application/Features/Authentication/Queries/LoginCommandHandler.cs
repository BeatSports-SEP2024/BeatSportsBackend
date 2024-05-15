using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Features.Authetication.Queries;
using BeatSportsAPI.Application.Models.Authentication;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Authentication.Queries;
public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginModelResponse>
{
    private readonly IMapper _mapper;
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IMapper mapper, IBeatSportsDbContext beatSportsDbContext)
    {
        _mapper = mapper;
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<LoginModelResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var existedUser = await _beatSportsDbContext.Accounts
            .FirstOrDefaultAsync(user => user.UserName == request.Username && user.Password == request.Password);

        if (existedUser == null)
        {
            throw new NotFoundException("This user does not exist");
        }

        var loginRequest = _mapper.Map<LoginModelRequest>(existedUser);

        // Trả về một chuỗi là AccessToken từ phương thức AuthenticateAsync
        string accessToken = await _identityService.AuthenticateAsync(loginRequest);

        var response = _mapper.Map<LoginModelResponse>(existedUser);
        response.Token = accessToken;  // Đảm bảo accessToken là kiểu string

        return response;
    }
}
