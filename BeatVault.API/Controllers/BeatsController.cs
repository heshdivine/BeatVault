using AutoMapper; // Import this
using BeatVault.API.DTOs;
using BeatVault.API.Entities;
using BeatVault.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BeatVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BeatsController : ControllerBase
    {
        private readonly IBeatRepository _beatRepository;
        private readonly IMapper _mapper; // Add Mapper

        // Inject Mapper here
        public BeatsController(IBeatRepository beatRepository, IMapper mapper)
        {
            _beatRepository = beatRepository;
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
    }
}