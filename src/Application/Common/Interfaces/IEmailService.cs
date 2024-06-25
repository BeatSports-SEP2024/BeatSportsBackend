using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Common.Interfaces;
public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string html);
}