using System.Globalization;
using System.Windows.Data;

namespace SudokuUI.Infrastructure;

public class MissingDigitsToSizeConverter : IValueConverter
{
    public int NormalSize { get; set; } = 0;
    public int ScaledSize { get; set; } = 0;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is int missing && missing <= 0 ? ScaledSize : NormalSize;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
