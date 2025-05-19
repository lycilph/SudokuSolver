using System.Windows;
using System.Windows.Controls;

namespace ImageImporterUI;

public static class TextBoxUtils
{
    public static bool GetScrollToEnd(DependencyObject obj)
    {
        return (bool)obj.GetValue(ScrollToEndProperty);
    }

    public static void SetScrollToEnd(DependencyObject obj, bool value)
    {
        obj.SetValue(ScrollToEndProperty, value);
    }

    public static readonly DependencyProperty ScrollToEndProperty =
        DependencyProperty.RegisterAttached(
            "ScrollToEnd", 
            typeof(bool), 
            typeof(TextBoxUtils), 
            new PropertyMetadata(false, ScrollToEndChanged));

    private static void ScrollToEndChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is TextBox tb)
        {
            if (e.NewValue is bool scroll_to_end && scroll_to_end)
            {
                tb.ScrollToEnd();
                tb.TextChanged += TextChanged;
            }
            else
                tb.TextChanged -= TextChanged;
        }
        else
            throw new InvalidOperationException("The attached ScrollToEnd property can only be applied to TextBox instances");
    }

    private static void TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox tb)
            tb.ScrollToEnd();
    }
}
