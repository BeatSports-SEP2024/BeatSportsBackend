using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Common.Exceptions;
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {

    }
}