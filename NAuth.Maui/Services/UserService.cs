using NAuth.ACL;
using NAuth.DTO;
using NAuth.DTO.User;

namespace NAuth.Maui.Services
{
    public class UserService : IUserService
    {
        private readonly UserClient _userClient;
        private readonly IAuthService _authService;

        public UserService(UserClient userClient, IAuthService authService)
        {
            _userClient = userClient;
            _authService = authService;
        }

        public async Task<UserInfo> RegisterAsync(UserInfo user)
        {
            return await _userClient.InsertAsync(user);
        }

        public async Task<UserInfo> UpdateProfileAsync(UserInfo user)
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("Token não encontrado");

            return await _userClient.UpdateAsync(user, token);
        }

        public async Task<bool> ChangePasswordAsync(string oldPassword, string newPassword)
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("Token não encontrado");

            var changeParam = new ChangePasswordParam
            {
                OldPassword = oldPassword,
                NewPassword = newPassword
            };

            return await _userClient.ChangePasswordAsync(changeParam, token);
        }

        public async Task<bool> SendPasswordRecoveryAsync(string email)
        {
            return await _userClient.SendRecoveryMailAsync(email);
        }

        public async Task<bool> ResetPasswordAsync(string hash, string newPassword)
        {
            var changeParam = new ChangePasswordUsingHashParam
            {
                RecoveryHash = hash,
                NewPassword = newPassword
            };

            return await _userClient.ChangePasswordUsingHashAsync(changeParam);
        }

        public async Task<string> UploadProfileImageAsync(Stream imageStream, string fileName)
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("Token não encontrado");

            return await _userClient.UploadImageUserAsync(imageStream, fileName, token);
        }

        public async Task<bool> HasPasswordAsync()
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("Token não encontrado");

            return await _userClient.HasPasswordAsync(token);
        }
    }
}
