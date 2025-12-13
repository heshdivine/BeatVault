using AutoMapper; // Import this
using BeatVault.API.Data.Repositories;
using BeatVault.API.DTOs;
using BeatVault.API.Entities;
using BeatVault.API.Interfaces;
using BeatVault.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeatVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BeatsController : ControllerBase
    {
        private readonly IBeatRepository _beatRepository;
        private readonly IMapper _mapper; // Add Mapper
        private readonly IUserRepository _userRepository;
        private readonly IFileService _fileService;

        // Inject Mapper here
        public BeatsController(IBeatRepository beatRepository, IMapper mapper, IUserRepository userRepository, IFileService fileService)
        {
            _beatRepository = beatRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _fileService = fileService;
        }

        // GET: api/beats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BeatDto>>> GetBeats()
        {
            // 1. Get raw entities from DB
            var beats = await _beatRepository.GetAllBeatsAsync();

            // 2. Convert to DTOs
            var beatDtos = _mapper.Map<IEnumerable<BeatDto>>(beats);

            // 3. Return clean data
            return Ok(beatDtos);
        }




        // POST: api/beats
        [Authorize(Roles = "Producer")]
        [HttpPost]
        public async Task<ActionResult<BeatDto>> CreateBeat([FromForm] BeatUploadDto uploadDto)
        {
            // 1. Handle Audio Upload (Generic)
            // We simply ask to save the file. The implementation decides WHERE.
            var audioUrl = await _fileService.SaveFileAsync(uploadDto.AudioFile, "beatvault-audio");

            if (string.IsNullOrEmpty(audioUrl))
                return BadRequest("Failed to upload audio");

            // 2. Create the Beat Object
            var beat = new Beat
            {
                Title = uploadDto.Title,
                BPM = uploadDto.BPM,
                Key = uploadDto.Key,
                LeasePrice = uploadDto.LeasePrice,
                ProducerId = 1, // Placeholder for User ID
                UploadedDate = DateTime.UtcNow,
                AudioUrl = audioUrl // Store the URL (Local or Cloud)
            };

            // 3. Handle Image Upload (Generic)
            if (uploadDto.CoverImage != null)
            {
                var imageUrl = await _fileService.SaveFileAsync(uploadDto.CoverImage, "beatvault-images");
                beat.CoverImageUrl = imageUrl;
            }

            // 4. Save to Database
            await _beatRepository.AddBeatAsync(beat);

            if (await _beatRepository.SaveChangesAsync())
            {
                return CreatedAtAction(nameof(GetBeats), new { id = beat.Id }, _mapper.Map<BeatDto>(beat));
            }

            return BadRequest("Problem saving beat");
        }

        // ... GetBeats method remains the same

        [Authorize(Roles = "Producer")]
        [HttpGet("my-studio")]
        public async Task<ActionResult<IEnumerable<BeatDto>>> GetMyBeats()
        {
            // Interview Win: Extracting the user ID/Email from the Token directly
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            // We would fetch the producer record using email, then fetch their beats
            var user = await _userRepository.GetUserByEmailAsync(userEmail);
            var beats = await _beatRepository.GetBeatsByProducerAsync(user.Id);

            return Ok(_mapper.Map<IEnumerable<BeatDto>>(beats));
        }
    }
}