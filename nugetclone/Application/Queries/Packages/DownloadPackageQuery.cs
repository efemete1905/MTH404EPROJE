using Application.Core;
using Domain;
using MediatR;

namespace Application.Queries.Packages
{
    public class DownloadPackageQuery : IRequest<Result<NugetPackage>>
    {
        public string PackageName { get; set; }
        public string PackageVersion { get; set; }
    }
}