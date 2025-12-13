using BeatVault.API.Interfaces;

namespace BeatVault.API.Services
{
    public class LocalFileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocalFileService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            // 1. Create a unique filename
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";

            // 2. Get or create the web root path
            var webRootPath = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");

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

            // 6. Return the full URL
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            return $"{baseUrl}/{folderName}/{uniqueFileName}";
        }

        public Task<bool> DeleteFileAsync(string fileUrl)
        {
            // For local files, we can delete them if we parse the path
            // This is optional for your current stage
            return Task.FromResult(true);
        }
    }
}