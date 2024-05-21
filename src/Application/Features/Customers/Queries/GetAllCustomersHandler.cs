using System;
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

namespace BeatSportsAPI.Application.Features.Customers.Queries;
public class GetAllCustomersHandler : IRequestHandler<GetAllCustomersCommand, PaginatedList<CustomerResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;
    public GetAllCustomersHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }
    public async Task<PaginatedList<CustomerResponse>> Handle(GetAllCustomersCommand request, CancellationToken cancellationToken)
    {
        if (request.PageIndex <= 0 || request.PageSize <= 0)
        {
            throw new BadRequestException("Page index and page size cannot less than 0");
        }
        var response = await _beatSportsDbContext.Customers
            .Where(c => !c.IsDelete)
            .ProjectTo<CustomerResponse>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageIndex, request.PageSize);
        return response;
    }
}
