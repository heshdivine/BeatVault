using AutoMapper;
using BeatVault.API.DTOs;
using BeatVault.API.Entities;
using BeatVault.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BeatVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(IUserRepository userRepository, ITokenService tokenService, IMapper mapper)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            // 1. Check if user exists
            if (await _userRepository.GetUserByEmailAsync(registerDto.Email) != null)
                return BadRequest("Email is taken");

            // 2. Map DTO to User
            var user = _mapper.Map<User>(registerDto);

            // 3. Hash Password using BCrypt (Interview Win!)
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // 4. Save to DB
            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();

            // 5. Return User + Token
            return new UserDto
            {
                Username = user.Username,
                Token = _tokenService.CreateToken(user),
                Role = user.Role
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // 1. Find User
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
            if (user == null) return Unauthorized("Invalid Email");

            // 2. Verify Password (BCrypt does the magic comparison)
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!isPasswordValid) return Unauthorized("Invalid Password");

            // 3. Return User + Token
            return new UserDto
            {
                Username = user.Username,
                Token = _tokenService.CreateToken(user),
                Role = user.Role
            };
        }
    }
}