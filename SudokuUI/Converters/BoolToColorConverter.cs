using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SudokuUI.Converters;

public class BoolToColorConverter : IValueConverter
{
    public SolidColorBrush TrueColorBrush { get; set; } = Brushes.Red;
    public SolidColorBrush FalseColorBrush { get; set; } = Brushes.Black;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value is bool val && val == true) ? TrueColorBrush : FalseColorBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
