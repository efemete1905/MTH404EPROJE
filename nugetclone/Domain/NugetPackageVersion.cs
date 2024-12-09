using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    [Table("NugetPackageVersions")]
    public class NugetPackageVersion
    {
      public NugetPackageVersion()
    {
        // Constructor
    }
        
        public Guid VersionId { get; set; }

      
        public Guid NugetPackageId { get; set; }
        public NugetPackage NugetPackage { get; set; }

        public int CurrentVersionDownloadCount { get; set; }

        public string PackageVersion { get; set; }
        
    }
}