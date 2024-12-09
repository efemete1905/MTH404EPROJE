using Azure.Storage.Blobs;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Persistence;
using NuGet.Packaging;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Packages
{
    public class Create
    {
        public class Command : IRequest
        {
            public NugetPackage NugetPackage { get; set; }

            public NugetPackageVersion NugetPackageVersion { get; set; }
            
            public IFormFile FormFile { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly BlobServiceClient _blobServiceClient;

            public Handler(DataContext context, BlobServiceClient blobServiceClient)
            {
                _context = context;
                _blobServiceClient =blobServiceClient;

            }

           public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
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
            var Tags = nuspecReader.GetTags();
            var name=nuspecReader.GetIdentity().Id;
            if (request.NugetPackage.NugetPackageVersions == null)
        {
        request.NugetPackage.NugetPackageVersions = new List<NugetPackageVersion>();
        }

        
           
            request.NugetPackage.PackageName = name;
            request.NugetPackage.FileSize = request.FormFile.Length.ToString();
            request.NugetPackage.CreatedDate = DateTime.Now;
            //request.NugetPackage.PackageNameWithVersion = request.FormFile.FileName;
            
            
            
            request.NugetPackage.Authors = authors;
            request.NugetPackage.Tags = Tags;
            request.NugetPackage.TotalDownloadCount = 0;
            
            _context.NugetPackages.Add(request.NugetPackage);
            await _context.SaveChangesAsync();
            request.NugetPackageVersion.NugetPackageId = request.NugetPackage.NugetPackageId;
            request.NugetPackageVersion.PackageVersion = version;
            request.NugetPackageVersion.CurrentVersionDownloadCount = 0;

              //var nugetPackageVersion = new NugetPackageVersion
                //        {
                  //          VersionId = Guid.NewGuid(),
                    //        NugetPackageId = request.NugetPackage.NugetPackageId,
                      //      PackageVersion = version,
                       //     CurrentVersionDownloadCount = 0
                        //}//;

            request.NugetPackage.NugetPackageVersions.Add(request.NugetPackageVersion);
            _context.NugetPackageVersions.Add(request.NugetPackageVersion);
           await _context.SaveChangesAsync();
           
           
            var containerClient = _blobServiceClient.GetBlobContainerClient("nugetpackages");
            var blobClient = containerClient.GetBlobClient(request.FormFile.FileName);
            await blobClient.UploadAsync(stream, true);

        }
    }

    return Unit.Value;
}
        }
    }
}