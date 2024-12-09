using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    [Table("NugetPackages")]
    public class NugetPackage
    {
        

    public NugetPackage()
    {
        // Constructor
    }
    public Guid NugetPackageId { get; set; }
    public string? PackageName { get; set; }
   
    
    public string? Authors { get; set; }
    public string? Tags { get; set; }
    public string? FileSize { get; set; }

   
   public int TotalDownloadCount { get; set; }
    
    public DateTime CreatedDate { get; set; }

    public ICollection<NugetPackageVersion> NugetPackageVersions { get; set; }

}
        
    }
