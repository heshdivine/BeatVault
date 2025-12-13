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
        private readonly ICloudinaryService _cloudinaryService;

        // Inject Mapper here
        public BeatsController(IBeatRepository beatRepository, IMapper mapper, IUserRepository userRepository, IFileService fileService, ICloudinaryService cloudinaryService
            )
        {
            _beatRepository = beatRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _fileService = fileService;
            _cloudinaryService = cloudinaryService;
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
        [Authorize] // Only logged in users
        [HttpPost]
        public async Task<ActionResult<BeatDto>> CreateBeat([FromForm] BeatUploadDto uploadDto)
        {
            // 1. Handle Audio Upload (Required)
            var audioResult = await _cloudinaryService.AddAudioAsync(uploadDto.AudioFile);

            if (audioResult.Error != null)
                return BadRequest(audioResult.Error.Message);

            // 2. Create the Beat Object
            var beat = new Beat
            {
                Title = uploadDto.Title,
                BPM = uploadDto.BPM,
                Key = uploadDto.Key,
                LeasePrice = uploadDto.LeasePrice,
                ProducerId = 1, // Placeholder: Use User.Identity logic here in next step
                UploadedDate = DateTime.UtcNow,
                AudioUrl = audioResult.SecureUrl.AbsoluteUri // Store the Cloud URL!
            };

            // 3. Handle Image Upload (Optional)
            if (uploadDto.CoverImage != null)
            {
                var imageResult = await _cloudinaryService.AddPhotoAsync(uploadDto.CoverImage);
                if (imageResult.Error != null) return BadRequest(imageResult.Error.Message);

                beat.CoverImageUrl = imageResult.SecureUrl.AbsoluteUri;
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