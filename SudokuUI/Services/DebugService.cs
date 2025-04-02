using System.Reflection;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Core.Commands;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using SudokuUI.Infrastructure;
using SudokuUI.Messages;
using SudokuUI.ViewModels;
using SudokuUI.Views;

namespace SudokuUI.Services;

public partial class DebugService : ObservableRecipient, IRecipient<MainWindowLoadedMessage>
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private DebugWindow? debug_window = null;

    public DebugService()
    {
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public void ToggleDebugWindow()
    {
        if (debug_window != null)
            debug_window.Close();
        else
            ShowDebugWindow();
    }

    public void ShowDebugWindow()
    {
        if (debug_window != null)
            return;

        var main_win = App.Current.MainWindow;

        var vm = App.Current.Services.GetRequiredService<DebugViewModel>();
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

    public void Receive(MainWindowLoadedMessage message)
    {
        logger.Info("Received the main window loaded message");

        var assembly = Assembly.GetExecutingAssembly();
        var mode = BuildModeDetector.GetBuildMode(assembly);

        if (mode == BuildModeDetector.BuildMode.Debug)
            ShowDebugWindow();
    }
}
