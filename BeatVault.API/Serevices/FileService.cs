using BeatVault.API.Interfaces;

namespace BeatVault.API.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FileService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            // 1. Create a unique filename (e.g., "guid-beat.mp3")
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";

            // 2. Get or create the web root path
            var webRootPath = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");

            // Ensure wwwroot exists
            if (!Directory.Exists(webRootPath))
                Directory.CreateDirectory(webRootPath);

            // 3. Combine with the folder name
            var uploadsFolder = Path.Combine(webRootPath, folderName);

            // 4. Create directory if it doesn't exist
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 5. Save the file to disk
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // 6. Return the full URL so the frontend can play it
            // Result: https://localhost:7144/audio/unique_name.mp3
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            return $"{baseUrl}/{folderName}/{uniqueFileName}";
        }
    }
}