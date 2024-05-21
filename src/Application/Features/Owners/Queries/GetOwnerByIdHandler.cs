using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Exceptions;
using MediatR;

namespace BeatSportsAPI.Application.Features.Owners.Queries;
public class GetOwnerByIdHandler : IRequestHandler<GetOwnerByIdCommand, OwnerResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;

    public GetOwnerByIdHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }

    public Task<OwnerResponse> Handle(GetOwnerByIdCommand request, CancellationToken cancellationToken)
    {
        var isExistedOwner = _beatSportsDbContext.Owners
            .Where(o => o.Id == request.OwnerId && !o.IsDelete)
            .ProjectTo<OwnerResponse>(_mapper.ConfigurationProvider)
            .SingleOrDefault();
        if(isExistedOwner == null) 
        {
            throw new NotFoundException($"{request.OwnerId} is not existed");
        }
        return Task.FromResult(isExistedOwner);
    }
}
