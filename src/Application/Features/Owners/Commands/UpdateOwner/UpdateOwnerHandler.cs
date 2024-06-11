using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Feedbacks.Commands.UpdateFeedback;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Owners.Commands.UpdateOwner;
public class UpdateOwnerHandler : IRequestHandler<UpdateOwnerCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public UpdateOwnerHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<BeatSportsResponse> Handle(UpdateOwnerCommand request, CancellationToken cancellationToken)
    {
        // Check Owner
        var owner = _dbContext.Owners.Where(x => x.Id == request.OwnerId)
                                     .Include(x => x.Account).SingleOrDefault();

        if (owner == null || owner.IsDelete)
        {
            throw new BadRequestException($"Owner with Owner ID:{request.OwnerId} does not exist or have been delele");
        }

        owner.Account.Email = request.Email;
        owner.Account.FirstName = request.FirstName;
        owner.Account.LastName = request.LastName;
        owner.Account.DateOfBirth = request.DateOfBirth;
        owner.Account.Gender = request.Gender.ToString();
        owner.Account.PhoneNumber = request.PhoneNumber;
        owner.Account.ProfilePictureURL = request.ProfilePictureURL;
        owner.Account.Bio = request.Bio;

        _dbContext.Owners.Update(owner);
        _dbContext.SaveChanges();

        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Update Owner successfully!"
        });
    }
}
