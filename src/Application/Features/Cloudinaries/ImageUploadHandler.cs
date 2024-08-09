using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using MediatR;
using Services.CloudinaryServices;

namespace BeatSportsAPI.Application.Features.Cloudinaries;
public class ImageUploadHandler : IRequestHandler<ImageUploadCommand, string>
{
    private readonly Cloudinary _cloudinary;

    public ImageUploadHandler(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }

    public async Task<string> Handle(ImageUploadCommand request, CancellationToken cancellationToken)
    {
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(request.FileName, request.ImageStream),
            PublicId = Path.GetFileNameWithoutExtension(request.FileName),
            Overwrite = true,
            Transformation = new Transformation().Quality("auto").FetchFormat("auto") // Optional transformation
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams, cancellationToken);
        return uploadResult.SecureUrl.AbsoluteUri;
    }
}