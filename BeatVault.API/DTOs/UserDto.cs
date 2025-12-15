namespace BeatVault.API.DTOs
{
    public class UserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty; // The JWT
        public string Role { get; set; } = string.Empty;
    }
}