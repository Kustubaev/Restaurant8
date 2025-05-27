using Restaurant8.Models;

namespace Restaurant8.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}
