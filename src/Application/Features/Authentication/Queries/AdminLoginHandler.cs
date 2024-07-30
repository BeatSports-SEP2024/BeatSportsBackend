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
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Authentication.Queries;
public class AdminLoginHandler : IRequestHandler<AdminLoginModelRequest, LoginResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IIdentityService _identityService;

    public AdminLoginHandler(IBeatSportsDbContext beatSportsDbContext, IIdentityService identityService)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _identityService = identityService;
    }

    public async Task<LoginResponse> Handle(AdminLoginModelRequest request, CancellationToken cancellationToken)
    {
        var loginResponse = await _identityService.AuthenticateAsync(request);
        if (loginResponse == null)
        {
            throw new BadRequestException("An error occurred when processing the request");
        }

        var user = await _beatSportsDbContext.Accounts
            .Where(x => x.UserName == request.Username)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            throw new BadRequestException("User does not exist");
        }

        // Assuming role is stored in user entity directly
        if (user.Role != "Admin")
        {
            throw new UnauthorizedAccessException("User is not an admin");
        }

        var walletExist = await _beatSportsDbContext.Wallets
            .Where(w => w.AccountId == user.Id)
            .SingleOrDefaultAsync();

        return new LoginResponse
        {
            Message = "Login Successfully",
            AccessToken = loginResponse.AccessToken,
            RefreshToken = loginResponse.RefreshToken,
            UserInfo = new AccountResponseForLogin
            {
                Id = user.Id,
                AccountId = user.Id,
                FullName = user.FirstName + " " + user.LastName,
                Email = user.Email,
                WalletId = walletExist?.Id ?? Guid.Empty, // Assuming wallet may not exist
                Role = "Admin"
            }
        };
    }
}
