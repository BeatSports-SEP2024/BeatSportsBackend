using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Common.Exceptions;
public class BadRequestException : Exception
{
    public BadRequestException()
    : base()
    {
    }
    public BadRequestException(string message) : base(message)
    {
    }
    public BadRequestException(string message, Exception innerException)
    : base(message, innerException)
    {
    }

    public BadRequestException(string name, object key)
        : base($"Entity \"{name}\" ({key}) has error.")
    {
    }
}