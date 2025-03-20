using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace SudokuUI.Converters;

public class IndexToThicknessConverter : IValueConverter
{
    public int Size { get; set; } = 1;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var t = new Thickness(0, 0, Size, Size);

        if (value is int index)
        {
            var row = index / 3;
            var col = index % 3;

            if (row == 2)
                t.Bottom = 0;
            if (col == 2)
                t.Right = 0;
        }

        return t;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
