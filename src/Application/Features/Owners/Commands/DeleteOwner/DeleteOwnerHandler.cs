using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Feedbacks.Commands.DeleteFeedback;
using MediatR;

namespace BeatSportsAPI.Application.Features.Owners.Commands.DeleteOwner;
public class DeleteOwnerHandler : IRequestHandler<DeleteOwnerCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public DeleteOwnerHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    Task<BeatSportsResponse> IRequestHandler<DeleteOwnerCommand, BeatSportsResponse>.Handle(DeleteOwnerCommand request, CancellationToken cancellationToken)
    {
        //check Owner
        var owner = _dbContext.Owners.Where(x => x.Id == request.OwnerId).SingleOrDefault();
        if (owner == null || owner.IsDelete)
        {
            throw new BadRequestException($"Owner with Owner ID:{request.OwnerId} does not exist or have been delele");
        }
        owner.IsDelete = true;
        _dbContext.Owners.Update(owner);
        _dbContext.SaveChanges();

        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Delete Owner successfully!"
        });
    }
}
