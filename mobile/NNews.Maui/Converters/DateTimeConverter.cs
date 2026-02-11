using System.Globalization;

namespace NNews.Maui.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                var format = parameter as string ?? "dd MMM yyyy";
                return dateTime.ToString(format, new CultureInfo("pt-BR"));
            }
            return value?.ToString();
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
