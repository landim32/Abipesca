using NAuth.ACL;
using NAuth.DTO.User;

namespace NAuth.Maui.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserClient _userClient;
        private const string TokenKey = "auth_token";
        private const string UserKey = "current_user";

        public AuthService(UserClient userClient)
        {
            _userClient = userClient;
        }

        public async Task<UserInfo> LoginAsync(string email, string password)
        {
            var loginParam = new LoginParam
            {
                Email = email,
                Password = password
            };

            var usuario = await _userClient.LoginWithEmailAsync(loginParam);
            
            if (usuario != null && !string.IsNullOrEmpty(usuario.Token))
            {
                await SaveTokenAsync(usuario.Token);
                await SecureStorage.Default.SetAsync(UserKey, System.Text.Json.JsonSerializer.Serialize(usuario));
            }

            return usuario.User;
        }

        public async Task LogoutAsync()
        {
            SecureStorage.Default.Remove(TokenKey);
            SecureStorage.Default.Remove(UserKey);
            await Task.CompletedTask;
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }

        public async Task<string?> GetTokenAsync()
        {
            try
            {
                return await SecureStorage.Default.GetAsync(TokenKey);
            }
            catch
            {
                return null;
            }
        }

        public async Task SaveTokenAsync(string token)
        {
            await SecureStorage.Default.SetAsync(TokenKey, token);
        }

        public async Task<UserInfo?> GetCurrentUserAsync()
        {
            try
            {
                var userJson = await SecureStorage.Default.GetAsync(UserKey);
                if (string.IsNullOrEmpty(userJson))
                    return null;

                var tokenResult = System.Text.Json.JsonSerializer.Deserialize<UserTokenResult>(userJson);
                return tokenResult?.User;
            }
            catch
            {
                return null;
            }
        }
    }
}
