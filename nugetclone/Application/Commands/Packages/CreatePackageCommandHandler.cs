using Application.Core;
using Azure.Storage.Blobs;
using Domain;
using MediatR;
using NuGet.Packaging;
using Persistence;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Packages
{
    public class CreatePackageCommandHandler : IRequestHandler<CreatePackageCommand, Result<Unit>>
    {
        private readonly DataContext _context;
        private readonly BlobServiceClient _blobServiceClient;

        public CreatePackageCommandHandler(DataContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        public async Task<Result<Unit>> Handle(CreatePackageCommand request, CancellationToken cancellationToken)
        {
            if (request.FormFile == null || Path.GetExtension(request.FormFile.FileName) != ".nupkg")
            {
                return Result<Unit>.Failure("Invalid package file format. Only .nupkg files are allowed.");
            }

            try
            {
                using (var stream = request.FormFile.OpenReadStream())
                {
                    var reader = new PackageArchiveReader(stream);
                    var nuspecReader = reader.NuspecReader;

                    var version = nuspecReader.GetVersion().ToString();
                    var description = nuspecReader.GetDescription();
                    var authors = nuspecReader.GetAuthors();
                    var tags = nuspecReader.GetTags();
                    var name = nuspecReader.GetIdentity().Id;

                    // Mevcut bir paket olup olmadığını kontrol et
                    var existingPackage = _context.NugetPackages
                        .FirstOrDefault(p => p.PackageName == name);

                    if (existingPackage != null)
                    {
                        return Result<Unit>.Failure($"A package with the name '{name}' already exists. Did you mean to update it?");
                    }

                    var nugetPackage = new NugetPackage
                    {
                        PackageName = name,
                        FileSize = request.FormFile.Length.ToString(),
                        CreatedDate = DateTime.Now,
                        Authors = authors,
                        Tags = tags,
                        TotalDownloadCount = 0,
                        NugetPackageVersions = new List<NugetPackageVersion>()
                    };

                    _context.NugetPackages.Add(nugetPackage);
                    await _context.SaveChangesAsync();

                    var nugetPackageVersion = new NugetPackageVersion
                    {
                        NugetPackageId = nugetPackage.NugetPackageId,
                        PackageVersion = version,
                        CurrentVersionDownloadCount = 0
                    };

                    nugetPackage.NugetPackageVersions.Add(nugetPackageVersion);
                    _context.NugetPackageVersions.Add(nugetPackageVersion);
                    await _context.SaveChangesAsync();

                    var containerClient = _blobServiceClient.GetBlobContainerClient("nugetpackages");
                    var blobClient = containerClient.GetBlobClient(request.FormFile.FileName);
                    await blobClient.UploadAsync(stream, true);

                    return Result<Unit>.Success(Unit.Value);
                }
            }
            catch (Exception ex)
            {
                return Result<Unit>.Failure($"An error occurred while creating the package: {ex.Message}");
            }
        }
    }
}