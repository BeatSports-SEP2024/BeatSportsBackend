using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Infrastructure.Persistence;
using CloudinaryDotNet;
using dotenv.net;
using FirebaseAdmin;
using FluentValidation.AspNetCore;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Filters;
using WebAPI.Services;

namespace WebAPI;
public static class ConfigureServices
{
    public static IServiceCollection AddWebUIServices(this IServiceCollection services)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        // Set your Cloudinary credentials
        //=================================

        DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
        Cloudinary cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
        cloudinary.Api.Secure = true;
        services.AddSingleton(cloudinary);

        services.AddHttpContextAccessor();

        services.AddHealthChecks()
            .AddDbContextCheck<BeatSportsAPIDbContext>();

        services.AddControllersWithViews(options =>
            options.Filters.Add<ApiExceptionFilterAttribute>())
                .AddFluentValidation(x => x.AutomaticValidationEnabled = false);

        string relativePath = @"sep490-demo-firebase-adminsdk-x94qp-c859860ea6.json";
        string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile(filePath)
        });

        services.AddRazorPages();

        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        return services;
    }
}
