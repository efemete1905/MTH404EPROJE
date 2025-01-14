using Microsoft.EntityFrameworkCore;
using Domain;

namespace Persistence
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        // DbSet Properties
        public DbSet<NugetPackage> NugetPackages { get; set; }
        public DbSet<NugetPackageVersion> NugetPackageVersions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureNugetPackageEntity(modelBuilder);
            ConfigureNugetPackageVersionEntity(modelBuilder);
        }

        private static void ConfigureNugetPackageEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NugetPackage>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.NugetPackageId);

                // Properties
                entity.Property(e => e.PackageName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Authors)
                    .HasMaxLength(200);

                entity.Property(e => e.Tags)
                    .HasMaxLength(200);

                entity.Property(e => e.FileSize)
                    .HasMaxLength(50);

                entity.Property(e => e.TotalDownloadCount)
                    .IsRequired();

                entity.Property(e => e.CreatedDate)
                    .IsRequired();

                // Relationships
                entity.HasMany(e => e.NugetPackageVersions)
                    .WithOne(v => v.NugetPackage)
                    .HasForeignKey(v => v.NugetPackageId);
            });
        }

        private static void ConfigureNugetPackageVersionEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NugetPackageVersion>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.VersionId);

                // Properties
                entity.Property(e => e.PackageVersion)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CurrentVersionDownloadCount)
                    .IsRequired();

                entity.Property(e => e.NugetPackageId)
                    .IsRequired();
            });
        }
    }
}
