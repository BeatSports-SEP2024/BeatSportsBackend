﻿using AutoFilterer.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities;
using MediatR;

namespace BeatSportsAPI.Application.Features.Sports.Queries;
public class GetSportCategoriesHandler : IRequestHandler<GetSportCategoriesCommand, PaginatedList<SportCategoriesResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;
    public GetSportCategoriesHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }
    public async Task<PaginatedList<SportCategoriesResponse>> Handle(GetSportCategoriesCommand request, CancellationToken cancellationToken)
    {
        if (request.PageIndex <= 0 || request.PageSize <= 0)
        {
            throw new BadRequestException("Page index and page size cannot less than 0");
        }
        IQueryable<SportCategory> query = _beatSportsDbContext.SportsCategories
        .Where(tp => !tp.IsDelete);

        if (!string.IsNullOrEmpty(request.SportCategoryName.ToString()))
        {
            query = query.Where(tp => tp.Name == request.SportCategoryName.ToString());
        }

        var response = await query
            .ProjectTo<SportCategoriesResponse>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageIndex, request.PageSize);

        return response;
    }
}