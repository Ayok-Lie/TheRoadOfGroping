using Microsoft.AspNetCore.Mvc;
using RoadOfGroping.Core.Files;

namespace RoadOfGroping.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly FileHelper _fileHelper;

        public FileController(IWebHostEnvironment env)
        {
            _fileHelper = new FileHelper(env);
        }

        [HttpPost("upload")]
        public IActionResult UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "No file selected." });
            }

            var filename = _fileHelper.SaveFile(file);
            if (filename == null)
            {
                return BadRequest(new { error = "File upload failed." });
            }

            return Ok(new { message = "File uploaded successfully.", filename });
        }

        [HttpGet("download/{filename}")]
        public IActionResult DownloadFile(string filename)
        {
            var filePath = _fileHelper.GetFilePath(filename);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { error = "File not found." });
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, "application/octet-stream", filename);
        }
    }
}