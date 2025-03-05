using System.Globalization;
using System.Windows.Data;
using SudokuUI.Services;

namespace SudokuUI.Converters;

public class InputModeToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is SelectionService.Mode mode && mode == SelectionService.Mode.Hints;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool is_hint && is_hint ? SelectionService.Mode.Hints : SelectionService.Mode.Digits;
    }
}
