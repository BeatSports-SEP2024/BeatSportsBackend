using AutoMapper;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetCourtSubdivisionById;
public class GetCourtSubdivisionByIdHandler : IRequestHandler<GetCourtSubdivisionByIdQuery, CourtSubdivisionV5>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetCourtSubdivisionByIdHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<CourtSubdivisionV5> Handle(GetCourtSubdivisionByIdQuery request, CancellationToken cancellationToken)
    {
        var currentDate = DateTime.Now;
        var query = _dbContext.CourtSubdivisions
            .Where(cs => !cs.IsDelete && cs.Id == request.CourtSubdivisionId)
            .Include(cs => cs.TimeCheckings)
            .Include(cs => cs.CourtSubdivisionSettings)
            .ThenInclude(css => css.SportCategories);

        var result = await query.Select(c => new CourtSubdivisionV5
        {
            CourtSubId = c.Id,
            BasePrice = c.BasePrice,
            Address = c.Court.Address,
            OwnerFullName = c.Court.Owner.Account.FirstName + " " + c.Court.Owner.Account.LastName,
            CourtSubName = c.CourtSubdivisionName,
            CourtType = c.CourtSubdivisionSettings.CourtType,
            CourtDescription = c.Court.Description,
            CourtSubDescription = c.CourtSubdivisionDescription,
            ImgUrls = ImageUrlSplitter.SplitImageUrls(c.Court.ImageUrls),
            SportCategories = c.CourtSubdivisionSettings.SportCategories.Name,
            Status = c.CreatedStatus.Equals(CourtSubdivisionCreatedStatus.Pending.ToString()) ? "Pending" :
                    !c.IsActive ? "Đang bảo trì" :
                    !c.TimeCheckings.Any() ? "Chưa có lịch" :
                    (c.TimeCheckings.Any(tc => tc.StartTime <= currentDate && tc.EndTime >= currentDate) ? "Đang cho thuê" :
                    (c.TimeCheckings.Any(tc => tc.StartTime > currentDate) ? "Đã đặt" : "Không có sử dụng"))
        }).FirstOrDefaultAsync(cancellationToken);

        return result;
    }
}