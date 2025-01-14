using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Packages
{
    public class CreatePackageCommand : IRequest<Unit>
    {
        public IFormFile FormFile { get; set; }
    }
}