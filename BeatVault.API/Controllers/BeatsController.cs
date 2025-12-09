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

        public BeatsController(IBeatRepository beatRepository)
        {
            _beatRepository = beatRepository;
        }

        // GET: api/beats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Beat>>> GetBeats()
        {
            var beats = await _beatRepository.GetAllBeatsAsync();
            return Ok(beats);
        }

        // POST: api/beats (Temporary for testing)
        [HttpPost]
        public async Task<ActionResult<Beat>> CreateBeat(Beat beat)
        {
            // Ideally we use DTOs here, but for today let's just test the DB connection
            await _beatRepository.AddBeatAsync(beat);

            if (await _beatRepository.SaveChangesAsync())
            {
                return CreatedAtAction(nameof(GetBeats), new { id = beat.Id }, beat);
            }

            return BadRequest("Failed to save beat");
        }
    }
}