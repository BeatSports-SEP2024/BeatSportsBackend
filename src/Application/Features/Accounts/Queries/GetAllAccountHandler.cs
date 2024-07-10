using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFilterer.Extensions;
using AutoFilterer.Types;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            throw new BadRequestException("Page index and page size cannot be less than 0");
        }

        var query = _beatSportsDbContext.Accounts
            .Where(tp => !tp.IsDelete);

        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(tp => tp.UserName.ToLower().Contains(request.Search.ToLower()) 
                || tp.PhoneNumber.Contains(request.Search)
                || tp.Wallet.Id.ToString().Contains(request.Search));
        }

        if (request.Role != RoleEnums.All)
        {
            query = query.Where(tp => tp.Role.Equals(request.Role.ToString()));
        }

        if (request.StartDate.HasValue && request.EndDate.HasValue)
        {
            query = query.Where(tp => tp.Created.Date >= request.StartDate.Value.Date && tp.Created.Date <= request.EndDate.Value.Date);
        }
        else if (request.StartDate.HasValue)
        {
            query = query.Where(tp => tp.Created.Date >= request.StartDate.Value.Date);
        }
        else if (request.EndDate.HasValue)
        {
            query = query.Where(tp => tp.Created.Date <= request.EndDate.Value.Date);
        }

        query = query.OrderByDescending(tp => tp.Created);

        // Thực hiện truy vấn với ProjectTo và PaginatedListAsync
        var response = await query
            .ProjectTo<AccountResponse>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageIndex, request.PageSize);

        return response;
    }
}