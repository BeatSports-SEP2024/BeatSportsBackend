//using BeatSportsAPI.Application.Common.Interfaces;
//using BeatSportsAPI.Application.Common.Response;
//using BeatSportsAPI.Application.Common.Exceptions;
//using MediatR;

//namespace BeatSportsAPI.Application.Features.Courts.CourtSportCategory.Commands;
//public class DeleteCourtSportCategoryHandler : IRequestHandler<DeleteCourtSportCategoryCommand, BeatSportsResponse>
//{
//    private readonly IBeatSportsDbContext _beatSportsDbContext;
//    public DeleteCourtSportCategoryHandler(IBeatSportsDbContext beatSportsDbContext)
//    {
//        _beatSportsDbContext = beatSportsDbContext;
//    }
//    public async Task<BeatSportsResponse> Handle(DeleteCourtSportCategoryCommand request, CancellationToken cancellationToken)
//    {
//        var updateCourtSport = _beatSportsDbContext.CourtSportCategories
//            .Where(t => t.CourtSubdivisionId == request.CourtId 
//            && t.SportCategoryId == request.SportCategoriesId)
//            .FirstOrDefault();
//        if(updateCourtSport == null)
//        {
//            throw new NotFoundException("Object not existed");
//        }
//        _beatSportsDbContext.CourtSportCategories.Remove(updateCourtSport);
//        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);
//        return new BeatSportsResponse
//        {
//            Message = "Delete Successfully"
//        };
//    }
//}
