using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SudokuUI.Infrastructure;

public class MissingDigitsToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is int missing && missing <= 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
