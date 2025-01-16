using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application;
using Azure.Storage.Blobs;
using System.Text.Json.Serialization;
using DotNetEnv;
using API.Middleware;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Threading.Tasks;

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

    // Configure Blob Storage using SAS token
    string blobServiceEndpoint = "https://mynugetpackages.blob.core.windows.net/nugetpackages";
    string sasToken = "sp=racwdli&st=2025-01-15T23:05:38Z&se=2025-02-16T07:05:38Z&spr=https&sv=2022-11-02&sr=c&sig=Z6VmLuBp5kzYzT9DY3X96R36WiBVUbCmt9w4XF7J57U%3D";
    var blobServiceClient = new BlobServiceClient(new Uri($"{blobServiceEndpoint}?{sasToken}"));
    builder.Services.AddSingleton(blobServiceClient);
}

async Task ConfigureMiddleware(WebApplication app)
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

    using var scope=app.Services.CreateScope();
    var services=scope.ServiceProvider;

    try{
        var context=services.GetRequiredService<DataContext>();
        await context.Database.MigrateAsync();
        await Seed.SeedData(context);
    }catch(Exception ex){
        var logger=services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex,"An error occured during migration");
    }

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