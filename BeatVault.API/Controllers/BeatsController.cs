using AutoMapper; // Import this
using BeatVault.API.Data.Repositories;
using BeatVault.API.DTOs;
using BeatVault.API.Entities;
using BeatVault.API.Interfaces;
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

        // Inject Mapper here
        public BeatsController(IBeatRepository beatRepository, IMapper mapper, IUserRepository userRepository)
        {
            _beatRepository = beatRepository;
            _userRepository = userRepository;
            _mapper = mapper;
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
        public async Task<ActionResult<BeatDto>> CreateBeat(Beat beat)
        {
            // Note: In the next phase, we will use a 'CreateBeatDto' here too!
            await _beatRepository.AddBeatAsync(beat);

            if (await _beatRepository.SaveChangesAsync())
            {
                // Return the DTO, not the Entity
                var beatDto = _mapper.Map<BeatDto>(beat);
                return CreatedAtAction(nameof(GetBeats), new { id = beat.Id }, beatDto);
            }

            return BadRequest("Failed to save beat");
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