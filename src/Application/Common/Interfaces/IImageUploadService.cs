using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Common.Interfaces;
public interface IImageUploadService
{
    Task<string> UploadImage(string fileImg, string base64string);
}