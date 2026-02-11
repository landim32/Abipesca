using NAuth.Maui.Services;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace Abipesca.Main.Handlers
{
    public class AuthenticationHandler : DelegatingHandler
    {
        private readonly IAuthService _authService;

        public AuthenticationHandler(IAuthService authService)
        {
            _authService = authService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Get token from AuthService
            var token = await GetTokenFromStorageAsync();
            
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<string?> GetTokenFromStorageAsync()
        {
            try
            {
                // Get the UserTokenResult from secure storage
                var userJson = await SecureStorage.Default.GetAsync("current_user");
                if (string.IsNullOrEmpty(userJson))
                    return null;

                // Deserialize to get the token
                var tokenResult = System.Text.Json.JsonSerializer.Deserialize<UserTokenResult>(userJson);
                return tokenResult?.Token;
            }
            catch
            {
                return null;
            }
        }

        // Helper class to deserialize the UserTokenResult
        private class UserTokenResult
        {
            [JsonPropertyName("token")]
            public string? Token { get; set; }
        }
    }
}
