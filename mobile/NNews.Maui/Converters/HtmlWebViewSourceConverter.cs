using System.Globalization;

namespace NNews.Maui.Converters
{
    public class HtmlWebViewSourceConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string html && !string.IsNullOrEmpty(html))
            {
                return new HtmlWebViewSource
                {
                    Html = html
                };
            }

            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
