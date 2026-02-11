using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NAuth.ACL;
using NAuth.DTO.Settings;
using NAuth.Maui.Services;
using NAuth.Maui.ViewModels;
using NAuth.Maui.Views;
using NNews.ACL;
using NNews.DTO.Settings;
using NNews.Maui.Services;
using NNews.Maui.ViewModels;
using NNews.Maui.Views;
using System.Net.Http.Headers;
using Abipesca.Main.Handlers;

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

            // Register AuthenticationHandler as Transient
            builder.Services.AddTransient<AuthenticationHandler>();

            // Configure HttpClient for NAuth with UserAgent and default settings
            builder.Services.AddHttpClient<UserClient>((serviceProvider, client) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<NAuthSetting>>().Value;
                var logger = serviceProvider.GetRequiredService<ILogger<UserClient>>();
                
                var apiUrl = GetNAuthApiUrl();
                logger.LogInformation("Configuring NAuth API URL: {ApiUrl}", apiUrl);
                
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

            // Configure HttpClient for NNews ArticleClient with Authentication
            builder.Services.AddHttpClient<ArticleClient>((serviceProvider, client) =>
            {
                var apiUrl = GetNNewsApiUrl();
                var logger = serviceProvider.GetRequiredService<ILogger<ArticleClient>>();
                
                logger.LogInformation("Configuring NNews Article API URL: {ApiUrl}", apiUrl);
                
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<AuthenticationHandler>();

            // Configure HttpClient for NNews CategoryClient with Authentication
            builder.Services.AddHttpClient<CategoryClient>((serviceProvider, client) =>
            {
                var apiUrl = GetNNewsApiUrl();
                var logger = serviceProvider.GetRequiredService<ILogger<CategoryClient>>();
                
                logger.LogInformation("Configuring NNews Category API URL: {ApiUrl}", apiUrl);
                
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<AuthenticationHandler>();

            // Configure HttpClient for NNews TagClient with Authentication
            builder.Services.AddHttpClient<TagClient>((serviceProvider, client) =>
            {
                var apiUrl = GetNNewsApiUrl();
                var logger = serviceProvider.GetRequiredService<ILogger<TagClient>>();
                
                logger.LogInformation("Configuring NNews Tag API URL: {ApiUrl}", apiUrl);
                
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<AuthenticationHandler>();

            // Configure NAuth Settings
            builder.Services.Configure<NAuthSetting>(options =>
            {
                options.ApiUrl = GetNAuthApiUrl();
            });

            // Configure NNews Settings
            builder.Services.Configure<NNewsSetting>(options =>
            {
                options.ApiUrl = GetNNewsApiUrl();
            });

            // Register NAuth application services
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IUserService, UserService>();

            // Register NAuth ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<ForgotPasswordViewModel>();
            builder.Services.AddTransient<ResetPasswordViewModel>();
            builder.Services.AddTransient<ChangePasswordViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<MainViewModel>();

            // Register NNews ViewModels
            builder.Services.AddTransient<ArticleListViewModel>();
            builder.Services.AddTransient<ArticleDetailViewModel>();
            builder.Services.AddTransient<CategoryListViewModel>();
            builder.Services.AddTransient<TagListViewModel>();
            builder.Services.AddTransient<SearchViewModel>();

            // Register NAuth Views
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<ForgotPasswordPage>();
            builder.Services.AddTransient<ResetPasswordPage>();
            builder.Services.AddTransient<ChangePasswordPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<NAuth.Maui.Views.MainPage>();

            // Register NNews Views
            builder.Services.AddTransient<ArticleListPage>();
            builder.Services.AddTransient<ArticleDetailPage>();
            builder.Services.AddTransient<CategoryListPage>();
            builder.Services.AddTransient<TagListPage>();
            builder.Services.AddTransient<SearchPage>();

            // Register Services
            builder.Services.AddSingleton<INavigationService, NavigationService>();

            return builder.Build();
        }

        private static string GetNAuthApiUrl()
        {
#if ANDROID
            return DeviceInfo.DeviceType == DeviceType.Virtual 
                ? "http://10.0.2.2:5004" 
                : "http://192.168.1.100:5004";
#elif WINDOWS
            return "http://localhost:5004";
#else
            return "http://localhost:5004";
#endif
        }

        private static string GetNNewsApiUrl()
        {
#if ANDROID
            return DeviceInfo.DeviceType == DeviceType.Virtual 
                ? "http://10.0.2.2:5007" 
                : "http://192.168.1.100:5007";
#elif WINDOWS
            return "http://localhost:5007";
#else
            return "http://localhost:5007";
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
