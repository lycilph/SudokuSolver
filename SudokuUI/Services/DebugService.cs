using System.Windows;
using SudokuUI.ViewModels;
using SudokuUI.Views;

namespace SudokuUI.Services;

public class DebugService
{
    private DebugWindow? debug_window = null;

    public void ShowDebugWindow()
    {
        if (debug_window != null)
            return;

        var main_win = App.Current.MainWindow;

        var vm = new DebugViewModel();
        debug_window = new DebugWindow { DataContext = vm, Owner = main_win };

        PositionDebugWindow(main_win);

        main_win.LocationChanged += MainWindowLocationChanged;

        // Reset the reference when the window is closed
        debug_window.Closed += (s, e) =>
        {
            main_win.LocationChanged -= MainWindowLocationChanged;
            debug_window = null;
        };

        debug_window.Show();
    }

    private void PositionDebugWindow(Window? main_win)
    {
        if (debug_window == null || main_win == null) return;

        // Position the debug window
        debug_window.Top = main_win.Top;

        debug_window.Left = main_win.Left - debug_window.Width - 10;
        if (debug_window.Left < 0)
            debug_window.Left = 0;
    }

    private void MainWindowLocationChanged(object? sender, EventArgs e)
    {
        PositionDebugWindow(sender as Window);
    }
}
