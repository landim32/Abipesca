using NAuth.ACL;
using NAuth.DTO;
using NAuth.DTO.User;

namespace NAuth.Maui.Services
{
    public interface IAuthService
    {
        Task<UserInfo> LoginAsync(string email, string password);
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<string?> GetTokenAsync();
        Task SaveTokenAsync(string token);
        Task<UserInfo?> GetCurrentUserAsync();
    }
}
