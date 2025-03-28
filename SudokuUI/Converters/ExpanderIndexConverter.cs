using System.Globalization;
using System.Windows.Data;

namespace SudokuUI.Converters;

public class ExpanderIndexConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int active_index = (int)value;
        int this_index = int.Parse(parameter.ToString() ?? "-1");
        return active_index == this_index;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool is_expanded = (bool)value;
        int this_index = int.Parse(parameter.ToString() ?? "-1");
        return is_expanded ? this_index : -1; // -1 means none expanded
    }
}