using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SupermarketPOS.UI.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isActive)
        {
            return isActive 
                ? new SolidColorBrush(Color.FromRgb(76, 175, 80))  // Green
                : new SolidColorBrush(Color.FromRgb(244, 67, 54));  // Red
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
