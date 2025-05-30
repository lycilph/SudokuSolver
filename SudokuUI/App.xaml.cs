﻿using System.Windows;
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
        services.AddSingleton<ImageImportService>();

        // These view models are singletons because they are used by multiple view models
        services.AddSingleton<GridViewModel>();
        services.AddSingleton<SettingsViewModel>();

        // Register view models
        services.AddTransient<MainViewModel>();
        services.AddTransient<DigitSelectionViewModel>();
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
        settings_service.Load();

        // Initialize main window
        var vm = Services.GetService<MainViewModel>();
        var win = new MainWindow { DataContext = vm };

        win.Show();
    }
}