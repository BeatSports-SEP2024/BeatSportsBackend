using BeatSportsAPI.Infrastructure.Common;
using WebAPI;
using Hangfire;
using Microsoft.OpenApi.Models;
using BeatSportsAPI.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using BeatSportsAPI.Application;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Services.VnPay.Config;
using Microsoft.AspNetCore.SignalR;
using WebAPI.Controllers.ChatHubs;
using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;
using WebAPI.Controllers.Queries;
using BeatSportsAPI.Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using WebAPI.Controllers.ChatHubs;
using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;
using WebAPI.Controllers.Queries;
using BeatSportsAPI.Domain.Entities;
using Services.Momo.Config;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
                      });
});
// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebUIServices();
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});
builder.Services.Configure<JsonOptions>(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"),
                new Hangfire.SqlServer.SqlServerStorageOptions()
                {
                    //TODO: Change hangfire sql server option
                }));
builder.Services.AddHangfireServer();
builder.Services.AddHttpClient();
builder.Services.Configure<VnpayConfig>(
                builder.Configuration.GetSection(VnpayConfig.ConfigName));
builder.Services.Configure<MomoConfig>(
                builder.Configuration.GetSection(MomoConfig.ConfigName));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = configuration["JWT:Issuer"],
        ValidAudience = configuration["JWT:Audience"]
    };
});

//SignalR
builder.Services.AddSignalR();

//GraphQl
builder.Services.AddGraphQLServer()
    .AddQueryType<QueryDatas>()
    .AddType<Account>();

// Configure Redis connection
var redisConnectionString = GetJsonInAppSettingsExtension.GetJson("Redis:RedisConnectionStrings");
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddScoped<StackExchange.Redis.IDatabase>(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new OpenApiInfo { Title = "BeatSportsAPI", Version = "v1" });
    config.DescribeAllParametersInCamelCase();
    config.EnableAnnotations();
    config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    config.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

    // Initialise and seed database
    using (var scope = app.Services.CreateScope())
    {
        //var initialiser = scope.ServiceProvider.GetRequiredService<BeatSportsAPIDbContextInitialiser>();
        //await initialiser.InitialiseAsync();
        //await initialiser.SeedAsync();
    }
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//SignalR Hub
app.MapHub<ChatHub>("chat-hub");

app.UseCors(MyAllowSpecificOrigins);
app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseAuthentication();
//app.UseIdentityServer();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapFallbackToFile("index.html"); ;

//Config Graphql
app.MapControllers();
app.MapGraphQL("/graphql");

app.Run();
