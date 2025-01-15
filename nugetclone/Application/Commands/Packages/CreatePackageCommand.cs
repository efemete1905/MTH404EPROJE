using Application.Core;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Packages
{
    public class CreatePackageCommand : IRequest<Result<Unit>>
    {
        public IFormFile FormFile { get; set; }
    }
}