using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Sports.Commands;
public class CreateSportCategoriesHandler : IRequestHandler<CreateSportCategoriesCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public CreateSportCategoriesHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<BeatSportsResponse> Handle(CreateSportCategoriesCommand request, CancellationToken cancellationToken)
    {
        var sportCategory = new SportCategory
        {
            Name = request.Name.ToString(),
            Description = request.Description,
            ImageURL = request.ImageURL,
        };

        _beatSportsDbContext.SportsCategories.Add(sportCategory);
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);

        return new BeatSportsResponse
        {
            Message = $"{request.Name} created successfully"
        };
    }
}
