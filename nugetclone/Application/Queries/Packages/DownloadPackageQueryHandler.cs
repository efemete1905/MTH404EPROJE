using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries.Packages
{
    public class DownloadPackageQueryHandler : IRequestHandler<DownloadPackageQuery, NugetPackage>
    {
        private readonly DataContext _context;
        private readonly BlobServiceClient _blobServiceClient;

        public DownloadPackageQueryHandler(DataContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        public async Task<NugetPackage> Handle(DownloadPackageQuery request, CancellationToken cancellationToken)
        {
            var nugetPackage = await _context.NugetPackages
                .Include(p => p.NugetPackageVersions)
                .FirstOrDefaultAsync(x => x.PackageName == request.PackageName, cancellationToken);

            if (nugetPackage == null)
            {
                throw new InvalidOperationException("Paket bulunamadı.");
            }

            var nugetPackageVersion = nugetPackage.NugetPackageVersions
                .FirstOrDefault(v => v.PackageVersion == request.PackageVersion);

            if (nugetPackageVersion == null)
            {
                throw new InvalidOperationException("Paket versiyonu bulunamadı.");
            }

            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient("nugetpackages");
            BlobClient blobClient = containerClient.GetBlobClient($"{request.PackageName}.{request.PackageVersion}.nupkg");
            BlobDownloadInfo blobDownloadInfo = await blobClient.DownloadAsync(cancellationToken);

            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string filePath = Path.Combine(userProfile, "Downloads", $"{request.PackageName}-{request.PackageVersion}.nupkg");

            using FileStream fs = File.OpenWrite(filePath);
            await blobDownloadInfo.Content.CopyToAsync(fs);

            nugetPackageVersion.CurrentVersionDownloadCount++;
            nugetPackage.TotalDownloadCount++;

            await _context.SaveChangesAsync(cancellationToken);

            return nugetPackage;
        }
    }
}