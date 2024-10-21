namespace Services.CloudinaryServices;
public interface ICloudinaryService
{
    public Task<string> UploadImageAsync(Stream imageStream, string fileName);
}
