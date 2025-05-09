﻿using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageImportUI;

public class ImageToBitmapConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Image<Rgb, byte> rgb)
            return rgb.ToBitmapSource();

        if (value is Image<Gray, byte> gray)
            return gray.ToBitmapSource();

        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
