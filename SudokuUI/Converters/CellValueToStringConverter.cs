using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Core.Models;

namespace SudokuUI.Converters;

public class CellValueToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Cell c)
            return c.IsFilled ? c.Value.ToString() : string.Empty;
        else if (value is int i)
            return i != 0 ? i.ToString() : string.Empty;
        else if (value is null)
            return string.Empty;

        return DependencyProperty.UnsetValue; // Return UnsetValue if the value is not of expected type
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
