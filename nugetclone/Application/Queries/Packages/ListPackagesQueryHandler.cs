using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;

namespace Application.Queries.Packages
{
    public class GetAllPackagesQueryHandler : IRequestHandler<ListPackagesQuery, Result<List<NugetPackage>>>
    {
        private readonly DataContext _context;

        public GetAllPackagesQueryHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<Result<List<NugetPackage>>> Handle(ListPackagesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var packages = await _context.NugetPackages
                    .Include(p => p.NugetPackageVersions) // İlişkili NugetPackageVersion nesnelerini yükle
                    .ToListAsync(cancellationToken);

                return Result<List<NugetPackage>>.Success(packages);
            }
            catch (Exception ex)
            {
                return Result<List<NugetPackage>>.Failure($"An error occurred while retrieving packages: {ex.Message}");
            }
        }
    }
}