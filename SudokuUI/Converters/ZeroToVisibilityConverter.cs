using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SudokuUI.Converters;

public class ZeroToVisibilityConverter : IValueConverter
{
    public Visibility HiddenState { get; set; } = Visibility.Hidden;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int digit && digit == 0)
            return HiddenState;

        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
