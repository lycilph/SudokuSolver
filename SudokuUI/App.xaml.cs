using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using SudokuUI.Services;
using SudokuUI.ViewModels;
using SudokuUI.Views;

namespace SudokuUI;

public partial class App : Application
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    public new static App Current => (App)Application.Current;
    public IServiceProvider Services { get; }

    public App()
    {
        logger.Info("Application starting...");

        Services = ConfigureServices();

        InitializeComponent();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Register singletons
        services.AddSingleton<PuzzleService>();
        services.AddSingleton<UndoRedoService>();
        services.AddSingleton<SelectionService>();
        services.AddSingleton<SettingsService>();
        services.AddSingleton<HighlightService>();
        services.AddSingleton<DebugService>();

        // Register view models
        services.AddTransient<MainViewModel>();
        services.AddTransient<GridViewModel>();
        services.AddTransient<DigitSelectionViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<SettingsOverlayViewModel>();
        services.AddTransient<WaitingOverlayViewModel>();

        return services.BuildServiceProvider();
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        // This needs to be instantiated somewhere...
        var highlighter = Services.GetService<HighlightService>();

        // Initialize main window
        var vm = Services.GetService<MainViewModel>();
        var win = new MainWindow { DataContext = vm };

        win.Show();
    }
}