using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;

namespace BeatSportsAPI.Application.Features.Jobs;
public class CheckTimeJob
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public CheckTimeJob(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }
    public void CheckTimeOfCourt()
    {
        var court = _beatSportsDbContext.Courts.ToList();
        foreach (var course in court)
        {
            //DateTime referenceDate = DateTime.Today;
            //DateTime startTime = referenceDate.Add(course.TimeStart);
            //DateTime endTime = referenceDate.Add(course.TimeEnd);

            //if (startTime <= DateTime.Now)
            //{
            //    course.IsDelete = true;
            //    _beatSportsDbContext.Courts.Update(course);
            //    _beatSportsDbContext.SaveChanges();
            //    Console.WriteLine($"Recurring job executed start for Court ${course.CourtName}");
            //}

            //if (endTime <= DateTime.Now)
            //{
            //    course.IsDelete = false;
            //    _beatSportsDbContext.Courts.Update(course);
            //    _beatSportsDbContext.SaveChanges();
            //    Console.WriteLine($"Recurring job executed end for Court ${course.CourtName}");
            //}

        }
    }
}
