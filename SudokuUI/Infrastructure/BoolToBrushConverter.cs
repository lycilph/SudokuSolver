using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SudokuUI.Infrastructure;

public class BoolToBrushConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length >= 3 &&
            values[0] is bool value &&
            values[1] is SolidColorBrush true_brush &&
            values[2] is SolidColorBrush false_brush)
        {
            return value ? true_brush : false_brush;
        }

        throw new InvalidOperationException();
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
