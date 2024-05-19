using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Features.Courts.TimePeriod.Command;
using FluentValidation;

namespace BeatSportsAPI.Application.Validators.TimePeriod;
public class CreateTimePeriodValidator : AbstractValidator<CreateTimePeriodCommand>
{
    public CreateTimePeriodValidator()
    {
        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required")
            .Must(BeAValidTime).WithMessage("End time must be a valid time")
            .GreaterThan(x => x.StartTime).WithMessage("End time must be greater than start time");
    }

    private bool BeAValidTime(TimeSpan time)
    {
        return time.TotalHours >= 0 && time.TotalHours < 24;
    }
}
