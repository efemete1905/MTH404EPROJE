using Domain;
using MediatR;

namespace Application.Queries.Packages
{
    public class DownloadPackageQuery : IRequest<NugetPackage>
    {
        public string PackageName { get; set; }
        public string PackageVersion { get; set; }
    }
}