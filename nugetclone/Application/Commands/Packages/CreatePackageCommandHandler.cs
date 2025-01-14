using Azure.Storage.Blobs;
using Domain;
using MediatR;
using NuGet.Packaging;
using Persistence;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Packages
{
    public class CreatePackageCommandHandler : IRequestHandler<CreatePackageCommand, Unit>
    {
        private readonly DataContext _context;
        private readonly BlobServiceClient _blobServiceClient;

        public CreatePackageCommandHandler(DataContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        public async Task<Unit> Handle(CreatePackageCommand request, CancellationToken cancellationToken)
        {
            if (request.FormFile != null && Path.GetExtension(request.FormFile.FileName) == ".nupkg")
            {
                // NuGet paketini açmak için PackageArchiveReader kullanılıyor
                using (var stream = request.FormFile.OpenReadStream())
                {
                    var reader = new PackageArchiveReader(stream);
                    var nuspecReader = reader.NuspecReader;

                    var version = nuspecReader.GetVersion().ToString();
                    var description = nuspecReader.GetDescription();
                    var authors = nuspecReader.GetAuthors();
                    var tags = nuspecReader.GetTags();
                    var name = nuspecReader.GetIdentity().Id;

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

                    return Unit.Value;
                }
            }

            throw new Exception("Invalid package file.");
        }
    }
}