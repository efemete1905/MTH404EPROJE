using MediatR;
using Microsoft.AspNetCore.Http;
using Domain;

namespace Application.Commands.Packages
{
    public class UpdatePackageVersionCommand : IRequest
    {
       
        public IFormFile FormFile { get; set; }
    }
}