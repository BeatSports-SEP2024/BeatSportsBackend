using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Common.Interfaces;
public interface IImageUploadService
{
    //2 parameters: Ten thu muc va kieu du lieu 2 phan cua anh
    Task<string> UploadImage(string fileImg, string base64string);
}