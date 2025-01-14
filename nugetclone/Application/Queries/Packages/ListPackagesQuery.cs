using Domain;
using MediatR;

namespace Application.Queries.Packages
{
    public class ListPackagesQuery : IRequest<List<NugetPackage>>
    {
        
    }
}