using MediatR;

namespace BeatSportsAPI.Application.Features.Cloudinaries;
public class ImageUploadCommand : IRequest<string>
{
    public Stream ImageStream { get; set; }
    public string FileName { get; set; }
}