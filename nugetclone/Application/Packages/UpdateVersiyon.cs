using Azure.Storage.Blobs;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Persistence;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Application.Packages
{
    public class UpdateVersion
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
                _blobServiceClient = blobServiceClient;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
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
                    throw new InvalidOperationException("Update Edilecek Paket Bulunamadı");
                }

                using (var stream = request.FormFile.OpenReadStream())
                {
                      var reader = new PackageArchiveReader(stream);
            
           
            var nuspecReader = reader.NuspecReader;

            var version = nuspecReader.GetVersion().ToString();
            var description = nuspecReader.GetDescription();
            var authors = nuspecReader.GetAuthors();
            var Tags = nuspecReader.GetTags();
            var name=nuspecReader.GetIdentity().Id;
            
                    
                    

                    foreach (var oldNugetPackage in oldNugetPackages)
                    {
                            //oldNugetPackage.PackageName = name;
                            //oldNugetPackage.FileSize = request.FormFile.Length.ToString();
                            //oldNugetPackage.CreatedDate = DateTime.Now;
                            //oldNugetPackage.PackageNameWithVersion = request.FormFile.FileName;
                            oldNugetPackage.Authors = authors;
                            oldNugetPackage.Tags = Tags;

                            request.NugetPackageVersion.CurrentVersionDownloadCount = 0;
                            request.NugetPackageVersion.PackageVersion = version;
                            request.NugetPackageVersion.NugetPackageId = oldNugetPackage.NugetPackageId;

                            
                           oldNugetPackage.NugetPackageVersions.Add(request.NugetPackageVersion);
                            _context.NugetPackageVersions.Add(request.NugetPackageVersion);
                            _context.NugetPackages.Update(oldNugetPackage);
                            }
                            

                            

                      await _context.SaveChangesAsync();

                       var blobClient = containerClient.GetBlobClient(request.FormFile.FileName);
                        await blobClient.UploadAsync(stream, true);

                        
                       

                        
                }

                return Unit.Value;
            }
        }
    }
}