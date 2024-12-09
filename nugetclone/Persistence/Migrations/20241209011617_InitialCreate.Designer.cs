﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Persistence;

#nullable disable

namespace Persistence.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20241209011617_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Domain.NugetPackage", b =>
                {
                    b.Property<Guid>("NugetPackageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Authors")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("FileSize")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("PackageName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Tags")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<int>("TotalDownloadCount")
                        .HasColumnType("int");

                    b.HasKey("NugetPackageId");

                    b.ToTable("NugetPackages");
                });

            modelBuilder.Entity("Domain.NugetPackageVersion", b =>
                {
                    b.Property<Guid>("VersionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("CurrentVersionDownloadCount")
                        .HasColumnType("int");

                    b.Property<Guid>("NugetPackageId")
                        .HasColumnType("char(36)");

                    b.Property<string>("PackageVersion")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("VersionId");

                    b.HasIndex("NugetPackageId");

                    b.ToTable("NugetPackageVersions");
                });

            modelBuilder.Entity("Domain.NugetPackageVersion", b =>
                {
                    b.HasOne("Domain.NugetPackage", "NugetPackage")
                        .WithMany("NugetPackageVersions")
                        .HasForeignKey("NugetPackageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("NugetPackage");
                });

            modelBuilder.Entity("Domain.NugetPackage", b =>
                {
                    b.Navigation("NugetPackageVersions");
                });
#pragma warning restore 612, 618
        }
    }
}
