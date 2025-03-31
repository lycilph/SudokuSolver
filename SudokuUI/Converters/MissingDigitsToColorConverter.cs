using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SudokuUI.Converters;

public class MissingDigitsToColorConverter : IValueConverter
{
    public Brush DisabledColor { get; set; } = Brushes.Black;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is int missing && missing < 0 ? DisabledColor : DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
