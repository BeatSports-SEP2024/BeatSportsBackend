using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;
using BeatSportsAPI.Application.Common.Exceptions;
using System.Reflection;

namespace BeatSportsAPI.Application.Features.Courts.TimePeriod.Command.UpdateTimePeriod;
public class UpdateTimePeriodHandler : IRequestHandler<UpdateTimePeriodCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public UpdateTimePeriodHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<BeatSportsResponse> Handle(UpdateTimePeriodCommand request, CancellationToken cancellationToken)
    {
        var updateTimePeriod = _beatSportsDbContext.TimePeriods
            .Where(t => t.Id == request.TimePeriodId && !t.IsDelete)
            .FirstOrDefault();
        if (updateTimePeriod == null)
        {
            throw new NotFoundException("Time Period does not existed");
        }
        foreach (PropertyInfo property in request.GetType().GetProperties())
        {
            var value = property.GetValue(request, null);
            if (value != null)
            {
                PropertyInfo propertyInfo = updateTimePeriod.GetType().GetProperty(property.Name);
                if (propertyInfo != null && propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(updateTimePeriod, value, null);
                }
            }
        }
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);
        return new BeatSportsResponse
        {
            Message = "Update Successfully"
        };
    }
}
