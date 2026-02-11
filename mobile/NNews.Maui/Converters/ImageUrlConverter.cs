using Microsoft.Extensions.Options;
using NNews.DTO.Settings;
using System.Globalization;

namespace NNews.Maui.Converters
{
    public class ImageUrlConverter : IValueConverter
    {
        private static string? _apiUrl;

        public static void SetApiUrl(string apiUrl)
        {
            _apiUrl = apiUrl?.TrimEnd('/');
        }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string imageName && !string.IsNullOrWhiteSpace(imageName))
            {
                if (imageName.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    imageName.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    return imageName;
                }

                return $"{_apiUrl}/images/{imageName}";
            }

            return parameter as string ?? "dotnet_bot.png";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
