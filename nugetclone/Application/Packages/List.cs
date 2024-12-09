using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Packages
{
    public class List
    {
        public class Query : IRequest<List<NugetPackage>>
        {

        }
        public class Handler : IRequestHandler<Query, List<NugetPackage>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<List<NugetPackage>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.NugetPackages
                    .Include(p => p.NugetPackageVersions) // İlişkili NugetPackageVersion nesnelerini yükle
                    .ToListAsync(cancellationToken);
            }
        }
        
    }
}