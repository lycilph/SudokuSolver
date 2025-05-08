using System.Globalization;
using System.Windows.Data;

namespace ImageImportUI;

public class RecognitionToBorderConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool recognition_failure && recognition_failure)
            return 1;

        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
