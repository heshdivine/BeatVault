namespace BeatVault.API.Interfaces
{
    public interface IFileService
    {
        // Returns the full URL of the saved file
        Task<string> SaveFileAsync(IFormFile file, string folderName);

        // Optional: Handle deletions
        Task<bool> DeleteFileAsync(string fileUrl);
    }
}