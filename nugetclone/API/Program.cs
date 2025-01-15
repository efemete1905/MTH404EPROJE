using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application;
using Azure.Storage.Blobs;
using System.Text.Json.Serialization;
using DotNetEnv;
using API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables
Env.Load("C:/Users/meteb_xm3wyja/Desktop/efec#proje/.env");

// Configure services
ConfigureServices(builder);

var app = builder.Build();

// Configure middleware
ConfigureMiddleware(app);

app.Run();

void ConfigureServices(WebApplicationBuilder builder)
{
    // Add controllers with JSON options
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

    // Add Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Configure database context
    var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
    builder.Services.AddDbContext<DataContext>(options =>
        options.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString),
            mySqlOptions => mySqlOptions.EnableRetryOnFailure(5))
            .EnableSensitiveDataLogging()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

    // Add MediatR
    builder.Services.AddMediatR(typeof(Application.Commands.Packages.CreatePackageCommandHandler).Assembly);

    // Configure Blob Storage
    var blobStorageConnectionString = Environment.GetEnvironmentVariable("BlobStorageConnectionString");
    var blobServiceClient = new BlobServiceClient(blobStorageConnectionString);
    builder.Services.AddSingleton(blobServiceClient);
}

void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    
    app.UseAuthorization();
    
    app.UseMiddleware<ExceptionMiddleware>();

    app.MapControllers();

    // Define custom endpoints
    app.MapGet("/search", (string searchTerm, DataContext context) =>
    {
        var packages = context.NugetPackages
            .Include(p => p.NugetPackageVersions)
            .Where(x => x.PackageName.Contains(searchTerm) ||
                        x.Tags.Contains(searchTerm) ||
                        x.Authors.Contains(searchTerm))
            .Select(x => new
            {
                x.PackageName,
                x.Authors,
                x.Tags,
                x.FileSize,
                x.TotalDownloadCount,
                x.CreatedDate,
                Versions = x.NugetPackageVersions.Select(v => new
                {
                    v.PackageVersion,
                    v.CurrentVersionDownloadCount,
                })
            }).ToList();

        return Results.Ok(packages);
    });
}