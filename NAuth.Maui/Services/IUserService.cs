using NAuth.ACL;
using NAuth.DTO;
using NAuth.DTO.User;

namespace NAuth.Maui.Services
{
    public interface IUserService
    {
        Task<UserInfo> RegisterAsync(UserInfo user);
        Task<UserInfo> UpdateProfileAsync(UserInfo user);
        Task<bool> ChangePasswordAsync(string oldPassword, string newPassword);
        Task<bool> SendPasswordRecoveryAsync(string email);
        Task<bool> ResetPasswordAsync(string hash, string newPassword);
        Task<string> UploadProfileImageAsync(Stream imageStream, string fileName);
        Task<bool> HasPasswordAsync();
    }
}
