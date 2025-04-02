using System.Windows;
using Core.Extensions;
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
        services.AddSingleton<SolverService>();
        services.AddSingleton<DebugService>();

        // The grid view model is a singleton because it is used by multiple view models
        services.AddSingleton<GridViewModel>();

        // Register view models
        services.AddTransient<MainViewModel>();
        services.AddTransient<DigitSelectionViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<OverlayViewModel>();
        services.AddTransient<VictoryViewModel>();
        services.AddTransient<HintsViewModel>();
        services.AddTransient<NewGameViewModel>();
        services.AddTransient<NotificationViewModel>();
        services.AddTransient<DebugViewModel>();

        return services.BuildServiceProvider();
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        // These needs to be instantiated somewhere to initialize properly...
        var highlighter = Services.GetService<HighlightService>();

        // Load settings
        var settings_service = Services.GetRequiredService<SettingsService>();
        settings_service.LoadSettings();

        // Initialize main window
        var vm = Services.GetService<MainViewModel>();
        var win = new MainWindow { DataContext = vm };

        win.Show();
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
        var puzzle_service = Services.GetRequiredService<PuzzleService>();
        puzzle_service.Serialize();
    }
}