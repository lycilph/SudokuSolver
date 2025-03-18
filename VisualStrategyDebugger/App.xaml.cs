using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using VisualStrategyDebugger.Service;
using VisualStrategyDebugger.ViewModels;
using VisualStrategyDebugger.Views;

namespace VisualStrategyDebugger;

public partial class App : Application
{
    public new static App Current => (App)Application.Current;
    public IServiceProvider Services { get; }

    public App()
    {
        Services = ConfigureServices();

        InitializeComponent();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Register singletons
        services.AddSingleton<GridService>();

        // Register view models
        services.AddTransient<MainViewModel>();
        services.AddTransient<GridViewModel>();
        services.AddTransient<CommandManagerViewModel>();
        services.AddTransient<StrategyManagerViewModel>();

        return services.BuildServiceProvider();
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var vm = Services.GetRequiredService<MainViewModel>();
        var win = new MainWindow { DataContext = vm };
        win.Show();
    }
}

