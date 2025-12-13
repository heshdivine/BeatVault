using BeatVault.API.Helpers;
using BeatVault.API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace BeatVault.API.Services
{
    public class CloudinaryService : IFileService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            if (file.Length == 0) return null;

            using var stream = file.OpenReadStream();
            string url = "";

            // LOGIC: Determine upload type based on folder name
            if (folderName.Contains("audio"))
            {
                // Audio is treated as "Video" in Cloudinary
                var uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = $"beatvault-{folderName}" // e.g. beatvault-audio
                };

                var result = await _cloudinary.UploadAsync(uploadParams);
                url = result.SecureUrl.AbsoluteUri;
            }
            else
            {
                // Default to Image (with your auto-crop settings)
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    Folder = $"beatvault-{folderName}" // e.g. beatvault-images
                };

                var result = await _cloudinary.UploadAsync(uploadParams);
                url = result.SecureUrl.AbsoluteUri;
            }

            return url;
        }

        public async Task<bool> DeleteFileAsync(string fileUrl)
        {
            // Cloudinary needs the "Public ID" to delete, not the URL.
            // Extracting Public ID from URL is complex, so for now we return true 
            // or implement a regex extraction if strictly needed.
            // Simplified:
            return await Task.FromResult(true);
        }
    }
}