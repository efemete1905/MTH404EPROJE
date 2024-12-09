using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application;
using Azure.Storage.Blobs;
using System.Text.Json.Serialization;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);
Env.Load("C:/Users/meteb_xm3wyja/Desktop/efec#proje/.env");


// Add services to the container.

builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler=ReferenceHandler.IgnoreCycles;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
builder.Services.AddDbContext<DataContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


builder.Services.AddMediatR(typeof(Application.Packages.List.Handler).Assembly);

// Azure Blob Storage Service'i ekleyin
var BlobStorageConnectionString = Environment.GetEnvironmentVariable("BlobStorageConnectionString");
var blobServiceClient = new BlobServiceClient(BlobStorageConnectionString);
builder.Services.AddSingleton(blobServiceClient);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/search", (string searchTerm, DataContext context) =>
{
   var packages = context.NugetPackages
   .Where(x=>
   x.PackageName.Contains(searchTerm)||
   x.Tags.Contains(searchTerm)||
x.Authors.Contains(searchTerm))
    .Select(x=> new
    {
        x.PackageName,
        
        x.Authors,
        x.Tags,
        x.FileSize,
        x.TotalDownloadCount,
        
        x.CreatedDate
    }).ToList();
    return Results.Ok(packages);
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
