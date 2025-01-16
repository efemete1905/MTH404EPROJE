using Microsoft.EntityFrameworkCore;
using Domain;
using System;

namespace Persistence
{
    public static class Seed
    {
        public static async Task SeedData(DataContext context)
        {
            if (context.NugetPackages.Any() || context.NugetPackageVersions.Any())
            {
                // Eğer veritabanında zaten veri varsa, seed işlemini yapma
                return;
            }

            // NugetPackage verileri
            var packages = new[] 
            {
                new NugetPackage
                {
                    NugetPackageId = Guid.Parse("08dd17ef-4a30-4398-8b51-1b0ab8ab7137"),
                    PackageName = "AppLogger",
                    Authors = "Author",
                    Tags = "",
                    FileSize = "3369",
                    TotalDownloadCount = 3,
                    CreatedDate = DateTime.Parse("2024-12-09T04:17:45.31548"),
                },
                new NugetPackage
                {
                    NugetPackageId = Guid.Parse("08dd341a-52cf-4a42-8d61-1af9690ee1fd"),
                    PackageName = "MathAddition",
                    Authors = "AuthorMath",
                    Tags = "",
                    FileSize = "3497",
                    TotalDownloadCount = 0,
                    CreatedDate = DateTime.Parse("2025-01-14T00:36:21.758632"),
                },
                new NugetPackage
                {
                    NugetPackageId = Guid.Parse("08dd3460-ee45-4502-8708-3103e06d649f"),
                    PackageName = "MathAddition",
                    Authors = "AuthorMath",
                    Tags = "",
                    FileSize = "3516",
                    TotalDownloadCount = 0,
                    CreatedDate = DateTime.Parse("2025-01-14T09:01:45.927959"),
                }
            };

            // NugetPackageVersion verileri
            var packageVersions = new[]
            {
                new NugetPackageVersion
                {
                    VersionId = Guid.Parse("08dd17f1-e322-447f-837b-c476e3b3e055"),
                    NugetPackageId = Guid.Parse("08dd17ef-4a30-4398-8b51-1b0ab8ab7137"),
                    CurrentVersionDownloadCount = 2,
                    PackageVersion = "1.0.1"
                },
                new NugetPackageVersion
                {
                    VersionId = Guid.Parse("08dd17ef-4a9b-4fbe-881b-bf42b4656c4f"),
                    NugetPackageId = Guid.Parse("08dd17ef-4a30-4398-8b51-1b0ab8ab7137"),
                    CurrentVersionDownloadCount = 1,
                    PackageVersion = "1.0.0"
                },
                new NugetPackageVersion
                {
                    VersionId = Guid.Parse("08dd3463-8e08-4355-8c37-9f88a54aa93e"),
                    NugetPackageId = Guid.Parse("08dd341a-52cf-4a42-8d61-1af9690ee1fd"),
                    CurrentVersionDownloadCount = 0,
                    PackageVersion = "1.0.3"
                },
                new NugetPackageVersion
                {
                    VersionId = Guid.Parse("08dd341a-5329-44be-8657-53b9b6b9766e"),
                    NugetPackageId = Guid.Parse("08dd341a-52cf-4a42-8d61-1af9690ee1fd"),
                    CurrentVersionDownloadCount = 0,
                    PackageVersion = "1.0.0"
                },
                new NugetPackageVersion
                {
                    VersionId = Guid.Parse("08dd3463-8e1b-4f37-8a67-aa089749ba7b"),
                    NugetPackageId = Guid.Parse("08dd3460-ee45-4502-8708-3103e06d649f"),
                    CurrentVersionDownloadCount = 0,
                    PackageVersion = "1.0.3"
                },
                new NugetPackageVersion
                {
                    VersionId = Guid.Parse("08dd3460-eebc-4493-895e-d06700d30290"),
                    NugetPackageId = Guid.Parse("08dd3460-ee45-4502-8708-3103e06d649f"),
                    CurrentVersionDownloadCount = 0,
                    PackageVersion = "1.0.2"
                }
            };

            // Yeni rastgele "Deneme" verisi
            var testPackage = new NugetPackage
            {
                NugetPackageId = Guid.NewGuid(),
                PackageName = "TestPackage",
                Authors = "TestAuthor",
                Tags = "test, sample",
                FileSize = new Random().Next(1000, 5000).ToString(),
                TotalDownloadCount = new Random().Next(0, 100),
                CreatedDate = DateTime.UtcNow
            };

            var testPackageVersion = new NugetPackageVersion
            {
                VersionId = Guid.NewGuid(),
                NugetPackageId = testPackage.NugetPackageId,
                CurrentVersionDownloadCount = new Random().Next(0, 100),
                PackageVersion = $"1.0.{new Random().Next(0, 10)}"
            };

            // Verileri veritabanına ekle
            context.NugetPackages.AddRange(packages);
            context.NugetPackageVersions.AddRange(packageVersions);
            context.NugetPackages.Add(testPackage);
            context.NugetPackageVersions.Add(testPackageVersion);

            await context.SaveChangesAsync();
        }
    }
}
