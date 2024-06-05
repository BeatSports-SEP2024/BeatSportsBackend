using System.Text.RegularExpressions;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Infrastructure.Common;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BeatSportsAPI.Infrastructure.Files;
public class ImageUploadService : IImageUploadService
{
    private readonly IConfiguration _configuration;
    private static string? ApiKey;
    private static string? Bucket;
    private static string? AuthEmail;
    private static string? AuthPassword;
    public ImageUploadService(IConfiguration configuration)
    {
        _configuration = configuration;
        ApiKey = GetJsonInAppSettingsExtension.GetJson("Firebase:ApiKey");
        Bucket = GetJsonInAppSettingsExtension.GetJson("Firebase:Bucket");
        AuthEmail = GetJsonInAppSettingsExtension.GetJson("Firebase:AuthEmail");
        AuthPassword = GetJsonInAppSettingsExtension.GetJson("Firebase:AuthPassword");
        Console.WriteLine(ApiKey, Bucket, AuthEmail, AuthPassword);
    }

    public bool ValidationImage(IFormFile file)
    {
        const int MAX_SIZE = 10 * 1024 * 1024; // 10MB
        string[] listExtensions = { ".png", ".jpeg", ".jpg", ".jfif", ".gif", ".webp" };

        bool isValid = false;

        if (file.Length == 0) throw new NullReferenceException("Null File");

        if (file.Length > 0 && file.Length < MAX_SIZE) isValid = true;

        if (isValid)
        {
            string extensionFile = Path.GetExtension(file.FileName);

            foreach (var extension in listExtensions)
            {
                if (extensionFile.Equals(extension))
                {
                    isValid = true;
                    break;
                }
            }
        }
        return isValid;
    }

    public async Task<string> UploadImage(string fileImg, string base64string)
    {
        if (string.IsNullOrWhiteSpace(base64string))
        {
            throw new BadRequestException("Image is null");
        }
        Stream stream = ConvertBase64ToStream(base64string);
        var filename = Guid.NewGuid();
        return await Upload(fileImg, filename, stream);
    }

    private async Task<string> Upload(string fileImg, Guid filename, Stream stream)
    {
        var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
        var isValidUser = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

        var cancellationToken = new CancellationTokenSource().Token;

        var task = new FirebaseStorage(Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(isValidUser.FirebaseToken),
                    ThrowOnCancel = true
                }).Child("assets").Child($"{fileImg}/{filename}").PutAsync(stream, cancellationToken);

        return (await task).ToString();
    }

    private Stream ConvertBase64ToStream(string base64)
    {
        base64 = base64.Trim();
        if ((base64.Length % 4 != 0) || !Regex.IsMatch(base64, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None)) throw new ArgumentException("Invalid image");
        byte[] bytes = Convert.FromBase64String(base64);
        return new MemoryStream(bytes);
    }
}
