using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Infrastructure.Files;
using BeatSportsAPI.Infrastructure.Files.Maps;
using BeatSportsAPI.Infrastructure.Identity;
using BeatSportsAPI.Infrastructure.Notification;
using BeatSportsAPI.Infrastructure.Persistence;
using BeatSportsAPI.Infrastructure.Persistence.Interceptors;
using BeatSportsAPI.Infrastructure.Services;
using BeatSportsAPI.Infrastructure.Services.SendEmail;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;
public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<BeatSportsAPIDbContext>(options =>
                options.UseInMemoryDatabase("BeatSportsAPIDb"));
        }
        else
        {
            services.AddDbContext<BeatSportsAPIDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    builder => builder.MigrationsAssembly(typeof(BeatSportsAPIDbContext).Assembly.FullName)));
        }

        services.AddScoped<IBeatSportsDbContext>(provider => provider.GetRequiredService<BeatSportsAPIDbContext>());
        services.AddScoped<BeatSportsAPIDbContextInitialiser>();

        services
            .AddDefaultIdentity<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<BeatSportsAPIDbContext>();

        //services
        //    .AddIdentity<IdentityUser, IdentityRole>()
        //    .AddEntityFrameworkStores<BeatSportsAPIDbContext>()
        //    .AddDefaultTokenProviders();

        //services.AddIdentityServer()
        //    .AddApiAuthorization<ApplicationUser, BeatSportsAPIDbContext>();

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<ICsvFileBuilder, CsvFileBuilder>();
        services.AddTransient<IImageUploadService, ImageUploadService>();
        services.AddTransient<INotificationService, NotificationService>();
        services.AddTransient<IEmailService, EmailService>();

        services.AddAuthentication()
            .AddIdentityServerJwt();

        services.AddAuthorization(options =>
            options.AddPolicy("CanPurge", policy => policy.RequireRole("Administrator")));

        return services;
    }
}
