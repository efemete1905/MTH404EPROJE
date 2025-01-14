using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries.Packages
{
    public class GetAllPackagesQueryHandler : IRequestHandler<ListPackagesQuery, List<NugetPackage>>
    {
        private readonly DataContext _context;

        public GetAllPackagesQueryHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<List<NugetPackage>> Handle(ListPackagesQuery request, CancellationToken cancellationToken)
        {
            return await _context.NugetPackages
                .Include(p => p.NugetPackageVersions) // İlişkili NugetPackageVersion nesnelerini yükle
                .ToListAsync(cancellationToken);
        }
    }
}