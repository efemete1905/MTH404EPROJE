using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    [Table("NugetPackageVersions")]
    public class NugetPackageVersion
    {
        // Constructor
        public NugetPackageVersion()
        {
            // Initialization logic if needed
        }

        // Properties
        public Guid VersionId { get; set; }
        public Guid NugetPackageId { get; set; }
        public NugetPackage NugetPackage { get; set; }
        public int CurrentVersionDownloadCount { get; set; }
        public string PackageVersion { get; set; }
    }
}