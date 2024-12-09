using Application.Packages;
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
        public NugetController(DataContext context)
        {
         //   _context = context;
        }
        [HttpPost]
        public async Task<ActionResult> CreateNugetPackage([FromForm(Name = "formFile")] IFormFile formFile)
        {
            var nugetPackage = new NugetPackage();
            var nugetPackageVersion = new NugetPackageVersion();
            return Ok(await Mediator.Send(new Create.Command {NugetPackage =nugetPackage, NugetPackageVersion = nugetPackageVersion,FormFile = formFile}));
        }

        [HttpGet]
        public async Task<ActionResult<List<NugetPackage>>> List()
        {
            return await Mediator.Send(new List.Query());
        }
         [HttpPost ("update")]
       public async Task<ActionResult> UpdateNugetPackage([FromForm(Name = "formFile")] IFormFile formFile)
        {
            
            var nugetPackage = new NugetPackage();
            var nugetPackageVersion = new NugetPackageVersion();
           
            return Ok(await Mediator.Send(new UpdateVersion.Command {NugetPackage =nugetPackage, NugetPackageVersion = nugetPackageVersion,FormFile = formFile}));
        }
        [HttpGet("download/{packageName}/{packageVersion}")]
        public async Task<ActionResult<NugetPackage>> Download(string packageName , string packageVersion)
        {
            return await Mediator.Send(new DownloadNuget.Query {PackageName = packageName, PackageVersion = packageVersion});
        }
    }
}


       
        
       
       
        
    //}
//} 