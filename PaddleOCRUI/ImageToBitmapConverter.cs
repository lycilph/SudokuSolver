using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PaddleOCRUI;

public class ImageToBitmapConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Mat mat && mat != null)
            return mat.ToBitmapSource();

        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}