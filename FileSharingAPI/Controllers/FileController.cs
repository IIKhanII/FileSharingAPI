using FileSharingAPI.Authentication;
using FileSharingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileSharingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private UsersContext db;
        private readonly IWebHostEnvironment _env;
        public FileController(UsersContext context, IWebHostEnvironment env)
        {
            db = context;
            _env = env;
        }

        [Authorize]
        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> UploadFile()
        {
            var files = Request.Form.Files;
            foreach (var file in files)
            {
                if(file.Length>0)
                {
                    string path = "Files/" + file.FileName;
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    FileModel fileModel = new FileModel { Id = Guid.NewGuid(), UserId = db.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name).Result.Id, Name = file.FileName, Path = path };
                    db.Files.Add(fileModel);
                    db.SaveChanges();
                }
            }
            return Ok(new Response { Status = "Success", Message = "File succesfully uploaded!" });
        }

        [Authorize]
        [HttpGet("download/{id}")]
        public FileResult DownloadFile(string id)
        {
            User user = db.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name || u.Phone == User.Identity.Name).Result;
            FileModel fileModel = db.Files.FirstOrDefaultAsync(u => u.Id == Guid.Parse(id) && u.UserId == user.Id).Result;
            FileStream fs = new FileStream(Path.Combine(_env.ContentRootPath, fileModel.Path), FileMode.Open);
            return File(fs, "application/octet-stream", fileModel.Name);
        }

        [Authorize]
        [HttpGet("{email}")]
        public IEnumerable<FileModel> Get(string email)
        {
            User user = db.Users.FirstOrDefaultAsync(u => u.Email == email).Result;
            var userFiles = db.Files.Where(u => u.UserId == user.Id).ToArray();
            if (userFiles != null)
                return Enumerable.Range(0, userFiles.Length).Select(index => new FileModel
                {
                    Id = userFiles[index].Id,
                    UserId = userFiles[index].UserId,
                    Name = userFiles[index].Name,
                    Path = userFiles[index].Path
                })
                .ToArray();
            else
                return null;
        }

    }
}
