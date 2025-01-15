using System.Reflection.Metadata;
using Application.Commands.Packages;
using Application.Queries.Packages;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Persistence;

namespace API.Controllers
{
   
    public class NugetController : BaseApiController
    {
       //private readonly DataContext _context;
       private readonly IMediator _mediator;
        public NugetController(IMediator Mediator)
        {
         
         _mediator = Mediator;
        }
        [HttpPost]
        public async Task<IActionResult> CreatePackage([FromForm] IFormFile formFile)
        {
            var request = new CreatePackageCommand { FormFile = formFile };
            
            return HandleResult(await _mediator.Send(request));
        }
        [HttpGet]
        public async Task<ActionResult<List<NugetPackage>>> GetAllPackages()
        {
            var query = new ListPackagesQuery();
            var packages = await _mediator.Send(query);
            return HandleResult(packages);   
        }
       [HttpPost("update-version")]
        public async Task<IActionResult> UpdatePackageVersion([FromForm] IFormFile formFile)
        {
            var request = new UpdatePackageVersionCommand { FormFile = formFile };
            return HandleResult(await _mediator.Send(request));
        }
        [HttpGet("download/{packageName}/{packageVersion}")]
        public async Task<ActionResult<NugetPackage>> Download(string packageName, string packageVersion)
        {
            return HandleResult(await _mediator.Send(new DownloadPackageQuery { PackageName = packageName, PackageVersion = packageVersion }));
        }
    }
}


       
        
       
       
        

 