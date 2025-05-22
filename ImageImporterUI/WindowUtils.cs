using System.ComponentModel;
using System.Windows;
using ImageImporterUI.ViewModels;

namespace ImageImporterUI;

public class WindowUtils
{
    public static bool GetWindowLifetimeEvents(DependencyObject obj)
    {
        return (bool)obj.GetValue(WindowLifetimeEventsProperty);
    }

    public static void SetWindowLifetimeEvents(DependencyObject obj, bool value)
    {
        obj.SetValue(WindowLifetimeEventsProperty, value);
    }

    public static readonly DependencyProperty WindowLifetimeEventsProperty =
        DependencyProperty.RegisterAttached(
            "WindowLifetimeEvents", 
            typeof(bool), 
            typeof(WindowUtils), 
            new PropertyMetadata(false, WindowLifetimeEventsChanged));

    private static void WindowLifetimeEventsChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is Window win)
        {
            if (e.NewValue is bool window_lifetime_events && window_lifetime_events)
            {
                win.ContentRendered += WindowContentRendered;
                win.Closing += WindowClosing;

                win.DataContextChanged += WindowDataContextChanged;
            }
        }
        else
            throw new InvalidOperationException("The attached WindowLifetimeEvents property can only be applied to Window instances");
    }

    private static void WindowDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is Window window && window.DataContext is IViewAware view_aware)
        {
            view_aware.OnRequestClose += (s, e) =>
            {
                window.DialogResult = e is bool return_value && return_value;
                window.Close(); 
            };
        }
    }

    private static void WindowContentRendered(object? sender, EventArgs e)
    {
        if (sender is Window window && window.DataContext is IViewAware view_aware)
        {
            view_aware.WindowContentRendered();

            window.ContentRendered -= WindowContentRendered;
        }
    }

    private static void WindowClosing(object? sender, CancelEventArgs e)
    {
        if (sender is Window window && window.DataContext is IViewAware view_aware)
        {
            view_aware.WindowClosing();

            window.Closing -= WindowClosing;
        }
    }
}
