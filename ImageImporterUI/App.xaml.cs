using System.Windows;
using ImageImporterUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ImageImporterUI;

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

        // Register singleton view models (These view models are singletons because they are used by multiple view models)

        // Register view models
        services.AddTransient<MainViewModel>();

        return services.BuildServiceProvider();
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        if (Current.MainWindow != null)
            return;

        // Initialize main window
        var vm = Services.GetService<MainViewModel>();
        var win = new MainWindow { DataContext = vm };

        win.Show();
    }
}

