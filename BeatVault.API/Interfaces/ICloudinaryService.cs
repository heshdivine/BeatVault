using CloudinaryDotNet.Actions;

namespace BeatVault.API.Interfaces
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        Task<VideoUploadResult> AddAudioAsync(IFormFile file); // Cloudinary treats Audio as "Video"
        Task<DeletionResult> DeleteFileAsync(string publicId);
    }
}