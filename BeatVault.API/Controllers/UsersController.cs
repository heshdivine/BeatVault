using BeatVault.API.Entities;
using BeatVault.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BeatVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            // 1. Check if user already exists
            if (await _userRepository.GetUserByEmailAsync(user.Email) != null)
            {
                return BadRequest("User with this email already exists.");
            }

            // 2. Add User (Note: In Week 2 we will Hash this password!)
            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null) return NotFound();

            return Ok(user);
        }
    }
}