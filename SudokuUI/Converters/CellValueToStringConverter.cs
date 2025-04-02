using System.Globalization;
using System.Windows.Data;
using Core.Models;

namespace SudokuUI.Converters;

public class CellValueToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Cell c && c.IsFilled ? c.Value.ToString() : string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
