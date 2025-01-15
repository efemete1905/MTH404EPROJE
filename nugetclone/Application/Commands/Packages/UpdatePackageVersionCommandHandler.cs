using Azure.Storage.Blobs;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using Persistence;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;

namespace Application.Commands.Packages
{
    public class UpdatePackageVersionCommandHandler : IRequestHandler<UpdatePackageVersionCommand, Result<Unit>>
    {
        private readonly DataContext _context;
        private readonly BlobServiceClient _blobServiceClient;

        public UpdatePackageVersionCommandHandler(DataContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        public async Task<Result<Unit>> Handle(UpdatePackageVersionCommand request, CancellationToken cancellationToken)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("nugetpackages");

            // Dosya adını ve paket adını ayırma
            string tempFileId = request.FormFile.FileName;
            int firstDotIndex = tempFileId.IndexOf('.');

            string fileId = firstDotIndex != -1 
                ? tempFileId.Substring(0, firstDotIndex) 
                : tempFileId;

            // Birden fazla NugetPackage nesnesini al
            var oldNugetPackages = await _context.NugetPackages
                .Include(p => p.NugetPackageVersions)
                .Where(x => x.PackageName == fileId)
                .ToListAsync();

            if (oldNugetPackages == null || oldNugetPackages.Count == 0)
            {
                return Result<Unit>.Failure("Update Edilecek Paket Bulunamadı");
            }

            using (var stream = request.FormFile.OpenReadStream())
            {
                var reader = new PackageArchiveReader(stream);
                var nuspecReader = reader.NuspecReader;

                var version = nuspecReader.GetVersion().ToString();
                var description = nuspecReader.GetDescription();
                var authors = nuspecReader.GetAuthors();
                var tags = nuspecReader.GetTags();
                var name = nuspecReader.GetIdentity().Id;

                foreach (var oldNugetPackage in oldNugetPackages)
                {
                    // Aynı versiyon olup olmadığını kontrol et
                    var existingVersion = oldNugetPackage.NugetPackageVersions
                        .FirstOrDefault(v => v.PackageVersion == version);

                    if (existingVersion != null)
                    {
                        return Result<Unit>.Failure($"A package with the name '{name}' and version '{version}' already exists. Update aborted.");
                    }

                    oldNugetPackage.Authors = authors;
                    oldNugetPackage.Tags = tags;

                    var nugetPackageVersion = new NugetPackageVersion
                    {
                        CurrentVersionDownloadCount = 0,
                        PackageVersion = version,
                        NugetPackageId = oldNugetPackage.NugetPackageId
                    };

                    oldNugetPackage.NugetPackageVersions.Add(nugetPackageVersion);
                    _context.NugetPackageVersions.Add(nugetPackageVersion);
                    _context.NugetPackages.Update(oldNugetPackage);
                }

                await _context.SaveChangesAsync();

                var blobClient = containerClient.GetBlobClient(request.FormFile.FileName);
                await blobClient.UploadAsync(stream, true);
            }

            return Result<Unit>.Success(Unit.Value);
        }
    }
}