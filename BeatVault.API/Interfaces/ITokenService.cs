using BeatVault.API.Entities;

namespace BeatVault.API.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}