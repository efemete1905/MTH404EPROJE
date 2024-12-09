using System.Reflection.Metadata;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Packages
{
    public class DownloadNuget
    {
        public class Query : IRequest<NugetPackage>
        {
            public string PackageName { get; set; }
            public string PackageVersion { get; set; }
        }

        public class Handler : IRequestHandler<Query, NugetPackage>
        {
            private readonly DataContext _context;
            private readonly BlobServiceClient _blobServiceClient;

            public Handler(DataContext context, BlobServiceClient blobServiceClient)
            {
                _context = context;
                _blobServiceClient = blobServiceClient;
            }

            public async Task<NugetPackage> Handle(Query request, CancellationToken cancellationToken)
            {
                var nugetPackage = await _context.NugetPackages
                    .Include(p => p.NugetPackageVersions)
                    .FirstOrDefaultAsync(x => x.PackageName == request.PackageName);
            
                var nugetPackageVersion = nugetPackage.NugetPackageVersions
                    .FirstOrDefault(v => v.PackageVersion == request.PackageVersion);
              
           
           
           
           
           
               BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient("nugetpackages");
               BlobClient blobClient = containerClient.GetBlobClient($"{request.PackageName}.{request.PackageVersion}.nupkg");
               BlobDownloadInfo blobDownloadInfo = await blobClient.DownloadAsync(cancellationToken);
             
             
             string filePath = Path.Combine(@"C:\Users\meteb_xm3wyja\Downloads", $"{request.PackageName}-{request.PackageVersion}.nupkg");
                using FileStream fs = File.OpenWrite(filePath);
                await blobDownloadInfo.Content.CopyToAsync(fs);
                
                nugetPackageVersion.CurrentVersionDownloadCount++;
                nugetPackage.TotalDownloadCount++;


                await _context.SaveChangesAsync(cancellationToken);

                return nugetPackage;
           
           
           
           
           
            }
        }
    }
}