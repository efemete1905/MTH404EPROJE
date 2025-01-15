using MediatR;
using Microsoft.AspNetCore.Http;
using Application.Core;

namespace Application.Commands.Packages
{
    public class UpdatePackageVersionCommand : IRequest<Result<Unit>>
    {
        public IFormFile FormFile { get; set; }
    }
}