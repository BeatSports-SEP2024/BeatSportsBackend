﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Accounts.Queries;
public class GetAllAccountHandler : IRequestHandler<GetAllAccountCommand, PaginatedList<AccountResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;
    public GetAllAccountHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }
    public async Task<PaginatedList<AccountResponse>> Handle(GetAllAccountCommand request, CancellationToken cancellationToken)
    {
        if (request.PageIndex <= 0 || request.PageSize <= 0)
        {
            throw new BadRequestException("Page index and page size cannot less than 0");
        }
        var response = await _beatSportsDbContext.Accounts
            .Where(tp => !tp.IsDelete && tp.Role.Equals(request.Role.ToString()))
            .ProjectTo<AccountResponse>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageIndex, request.PageSize);
        return response;
    }
}