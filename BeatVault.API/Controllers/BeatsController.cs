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
        public BeatsController(IBeatRepository beatRepository, IMapper mapper, IUserRepository userRepository, IFileService fileService )
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
            // 1. Basic Mapping
            var beat = new Beat
            {
                Title = uploadDto.Title,
                BPM = uploadDto.BPM,
                Key = uploadDto.Key,
                LeasePrice = uploadDto.LeasePrice,
                ProducerId = 1, // Placeholder
                UploadedDate = DateTime.UtcNow
            };

            // 2. Save Audio Locally
            // This will save to: BeatVault.API/wwwroot/audio/
            beat.AudioUrl = await _fileService.SaveFileAsync(uploadDto.AudioFile, "audio");

            // 3. Save Image Locally (Optional)
            if (uploadDto.CoverImage != null)
            {
                beat.CoverImageUrl = await _fileService.SaveFileAsync(uploadDto.CoverImage, "images");
            }

            // 4. Save to DB
            await _beatRepository.AddBeatAsync(beat);
            await _beatRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBeats), new { id = beat.Id }, _mapper.Map<BeatDto>(beat));
        }

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