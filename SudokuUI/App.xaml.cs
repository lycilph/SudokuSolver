using Microsoft.Extensions.DependencyInjection;
using NLog;
using SudokuUI.Services;
using SudokuUI.ViewModels;
using SudokuUI.Views;
using System.Windows;

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
        services.AddSingleton<SelectionService>();
        services.AddSingleton<HighlightService>();
        services.AddSingleton<GridViewModel>(); // This is needs to be a singleton as both the MainViewModel and the HighlightService needs this

        // Register view models
        services.AddTransient<MainViewModel>();
        services.AddTransient<SelectionViewModel>();
        services.AddTransient<SettingsViewModel>();

        return services.BuildServiceProvider();
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        // This needs to be instantiated somewhere...
        var highlighter = Services.GetService<HighlightService>();

        var vm = Services.GetService<MainViewModel>();
        var win = new MainWindow { DataContext = vm };

        win.Show();
    }
}