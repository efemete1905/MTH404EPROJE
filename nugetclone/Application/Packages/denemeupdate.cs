using Azure.Storage.Blobs;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Persistence;
using System;
using System.IO;
using System.Threading;
/*using System.Threading.Tasks;
using NuGet.Packaging;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.Extensions.Azure;

namespace Application.Packages
{
    public class denemeupdate
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
                request.NugetPackageVersion.CurrentVersionDownloadCount = 0;
                request.NugetPackageVersion.PackageVersion = version;
                request.NugetPackageVersion.NugetPackageId = oldNugetPackage.NugetPackageId;

                oldNugetPackage.NugetPackageVersions.Add(request.NugetPackageVersion);
            
                    
                    

                   

                return Unit.Value;
            }
        }
    }
}
}*/