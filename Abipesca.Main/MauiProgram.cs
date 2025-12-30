using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NAuth.ACL;
using NAuth.DTO.Settings;
using NAuth.Maui.Services;
using NAuth.Maui.ViewModels;
using NAuth.Maui.Views;
using System.Net.Http.Headers;

namespace Abipesca.Main
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
            builder.Logging.SetMinimumLevel(LogLevel.Debug);
#endif

            // Configure HttpClient with UserAgent and default settings
            builder.Services.AddHttpClient<UserClient>((serviceProvider, client) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<NAuthSetting>>().Value;
                var logger = serviceProvider.GetRequiredService<ILogger<UserClient>>();
                
                var apiUrl = GetApiUrl();
                logger.LogInformation("Configuring API URL: {ApiUrl}", apiUrl);
                
                client.BaseAddress = new Uri(apiUrl);
                
                // Build UserAgent with device fingerprint
                var userAgent = GetUserAgent();
                client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
                
                // Add device fingerprint as custom header
                var fingerprint = GetDeviceFingerprint();
                client.DefaultRequestHeaders.Add("X-Device-Fingerprint", fingerprint);
                
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromSeconds(30);
                
                logger.LogInformation("UserAgent: {UserAgent}", userAgent);
                logger.LogInformation("Fingerprint: {Fingerprint}", fingerprint);
            });

            // Configure default HttpClient for other services
            builder.Services.AddHttpClient();

            // Configure NAuth Settings
            builder.Services.Configure<NAuthSetting>(options =>
            {
                options.ApiUrl = GetApiUrl();
            });

            // Register application services
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IUserService, UserService>();

            // Register ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<ForgotPasswordViewModel>();
            builder.Services.AddTransient<ResetPasswordViewModel>();
            builder.Services.AddTransient<ChangePasswordViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<MainViewModel>();

            // Register Views
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<ForgotPasswordPage>();
            builder.Services.AddTransient<ResetPasswordPage>();
            builder.Services.AddTransient<ChangePasswordPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<NAuth.Maui.Views.MainPage>();

            return builder.Build();
        }

        private static string GetApiUrl()
        {
#if ANDROID
            // 10.0.2.2 é o IP especial do emulador Android para acessar localhost da máquina host
            return DeviceInfo.DeviceType == DeviceType.Virtual 
                ? "https://10.0.2.2:5005" 
                : "https://192.168.1.100:5005"; // Ajuste para seu IP de rede local
#elif WINDOWS
            return "https://localhost:5005";
#else
            return "https://localhost:5005";
#endif
        }

        private static string GetUserAgent()
        {
            var appVersion = AppInfo.Current.VersionString;
            var appBuild = AppInfo.Current.BuildString;
            var platform = DeviceInfo.Current.Platform.ToString();
            var deviceModel = DeviceInfo.Current.Model;
            var osVersion = DeviceInfo.Current.VersionString;
            var manufacturer = DeviceInfo.Current.Manufacturer;
            var deviceType = DeviceInfo.Current.DeviceType.ToString();
            
            return $"Abipesca/{appVersion} (MAUI; {platform} {osVersion}; {manufacturer} {deviceModel}; {deviceType})";
        }

        private static string GetDeviceFingerprint()
        {
            var platform = DeviceInfo.Current.Platform.ToString();
            var deviceModel = DeviceInfo.Current.Model;
            var osVersion = DeviceInfo.Current.VersionString;
            var manufacturer = DeviceInfo.Current.Manufacturer;
            
            // Generate a unique fingerprint based on device characteristics
            var fingerprintData = $"{platform}_{manufacturer}_{deviceModel}_{osVersion}";
            return GenerateFingerprint(fingerprintData);
        }

        private static string GenerateFingerprint(string data)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
            return Convert.ToHexString(hashBytes)[..16];
        }
    }
}
