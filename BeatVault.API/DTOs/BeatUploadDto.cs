using System.ComponentModel.DataAnnotations;

namespace BeatVault.API.DTOs
{
    public class BeatUploadDto
    {
        [Required] public string Title { get; set; } = string.Empty;
        [Required] public int BPM { get; set; }
        [Required] public string Key { get; set; } = string.Empty;
        [Required] public decimal? LeasePrice { get; set; }

        [Required] public IFormFile AudioFile { get; set; } = null!; // The MP3
        public IFormFile? CoverImage { get; set; } // The JPG
    }
}