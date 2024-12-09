using Microsoft.EntityFrameworkCore;
using Domain;

namespace Persistence
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<NugetPackage> NugetPackages { get; set; }
        public DbSet<NugetPackageVersion> NugetPackageVersions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // NugetPackage tablosu için yapılandırmalar
            modelBuilder.Entity<NugetPackage>(entity =>
            {
                entity.HasKey(e => e.NugetPackageId); // Primary key

                entity.Property(e => e.PackageName)
                    .IsRequired()
                    .HasMaxLength(100); // PaketId alanı zorunlu ve maksimum 100 karakter

                 

                entity.Property(e => e.Authors)
                    .HasMaxLength(200); // Authors alanı maksimum 200 karakter

                entity.Property(e => e.Tags)
                    .HasMaxLength(200); // Tags alanı maksimum 200 karakter

                entity.Property(e => e.FileSize)
                    .HasMaxLength(50); // FileSize alanı maksimum 50 karakter

               

                entity.Property(e => e.TotalDownloadCount)
                    .IsRequired(); // TotalDownloadCount alanı zorunlu

                entity.Property(e => e.CreatedDate)
                    .IsRequired(); // CreatedDate alanı zorunlu

                // One-to-Many ilişki
                entity.HasMany(e => e.NugetPackageVersions)
                    .WithOne(v => v.NugetPackage)
                    .HasForeignKey(v => v.NugetPackageId);
            });

            // NugetPackageVersion tablosu için yapılandırmalar
            modelBuilder.Entity<NugetPackageVersion>(entity =>
            {
                entity.HasKey(e => e.VersionId); // Primary key

                entity.Property(e => e.PackageVersion)
                    .IsRequired()
                    .HasMaxLength(50); // PaketVersiyon alanı zorunlu ve maksimum 50 karakter

                entity.Property(e => e.CurrentVersionDownloadCount)
                    .IsRequired(); // CurrentVersionDownloadCount alanı zorunlu

                entity.Property(e => e.NugetPackageId)
                    .IsRequired(); // NugetPackageId alanı zorunlu
            });
        }
    }
}