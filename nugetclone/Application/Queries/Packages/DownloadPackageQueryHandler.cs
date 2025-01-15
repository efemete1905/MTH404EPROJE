using Application.Core;
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
    public class DownloadPackageQueryHandler : IRequestHandler<DownloadPackageQuery, Result<NugetPackage>>
    {
        private readonly DataContext _context;
        private readonly BlobServiceClient _blobServiceClient;

        public DownloadPackageQueryHandler(DataContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        public async Task<Result<NugetPackage>> Handle(DownloadPackageQuery request, CancellationToken cancellationToken)
        {
            var nugetPackage = await _context.NugetPackages
                .Include(p => p.NugetPackageVersions)
                .FirstOrDefaultAsync(x => x.PackageName == request.PackageName, cancellationToken);

            if (nugetPackage == null)
            {
                return Result<NugetPackage>.Failure("Package not found.");
            }

            var nugetPackageVersion = nugetPackage.NugetPackageVersions
                .FirstOrDefault(v => v.PackageVersion == request.PackageVersion);

            if (nugetPackageVersion == null)
            {
                return Result<NugetPackage>.Failure("Package version not found.");
            }

            try
            {
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

                return Result<NugetPackage>.Success(nugetPackage);
            }
            catch (Exception ex)
            {
                return Result<NugetPackage>.Failure($"An error occurred while downloading the package: {ex.Message}");
            }
        }
    }
}