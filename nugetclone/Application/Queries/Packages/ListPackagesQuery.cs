using Domain;
using MediatR;
using Application.Core;
using System.Collections.Generic;

namespace Application.Queries.Packages
{
    public class ListPackagesQuery : IRequest<Result<List<NugetPackage>>>
    {
    }
}