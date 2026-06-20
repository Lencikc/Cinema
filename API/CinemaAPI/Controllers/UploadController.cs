using CinemaAPI.CustomAttributes;
using Microsoft.AspNetCore.Mvc;

namespace CinemaAPI.Controllers
{
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public UploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        [Route("/cinema/upload")]
        [RoleAuthorize([1])]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return new BadRequestObjectResult(new { status = false, message = "Файл не выбран" });
            if (file.Length > 5 * 1024 * 1024)
                return new BadRequestObjectResult(new { status = false, message = "Файл больше 5 МБ" });

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            if (!allowed.Contains(ext))
                return new BadRequestObjectResult(new { status = false, message = "Допустимы только jpg/png/webp/gif" });

            var dir = Path.Combine(_env.ContentRootPath, "uploads");
            Directory.CreateDirectory(dir);

            var name = Guid.NewGuid().ToString("N") + ext;
            var path = Path.Combine(dir, name);

            using (var stream = new FileStream(path, FileMode.Create))
                await file.CopyToAsync(stream);

            var url = $"http://localhost:5190/uploads/{name}";
            return new OkObjectResult(new { status = true, url });
        }
    }
}
