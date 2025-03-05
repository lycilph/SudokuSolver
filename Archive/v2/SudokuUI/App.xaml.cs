using Microsoft.Extensions.DependencyInjection;
using SudokuUI.Services;
using SudokuUI.ViewModels;
using System.Windows;

namespace SudokuUI;

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

        services.AddSingleton<SelectionService>();
        services.AddSingleton<PuzzleService>();

        services.AddTransient<MainViewModel>();

        return services.BuildServiceProvider();
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var vm = Services.GetService<MainViewModel>();
        var win = new MainWindow { DataContext = vm };

        win.Show();
    }
}

