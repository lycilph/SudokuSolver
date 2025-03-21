using CommunityToolkit.Mvvm.ComponentModel;
using ControlzEx.Theming;
using Core.Extensions;
using SudokuUI.Services;
using System.Collections.ObjectModel;

namespace SudokuUI.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly SettingsService settings_service;

    public bool IsOpen
    {
        get => settings_service.IsShown;
        set => settings_service.IsShown = value;
    }

    [ObservableProperty]
    private bool isLight = true;

    [ObservableProperty]
    private Theme selectedApplicationColor;

    public ObservableCollection<Theme> ApplicationColors { get; set; }

    public SettingsViewModel(SettingsService settings_service)
    {
        this.settings_service = settings_service;

        ApplicationColors = settings_service.Themes.ToObservableCollection();
        IsLight = settings_service.GetBaseColorScheme() == SettingsService.Light;
        SelectedApplicationColor = ApplicationColors.First(t => t.ColorScheme == settings_service.GetColorScheme());

        settings_service.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SettingsService.IsShown))
                OnPropertyChanged(nameof(IsOpen));
        };
    }

    partial void OnIsLightChanged(bool value)
    {
        var base_color_scheme = IsLight ? SettingsService.Light : SettingsService.Dark;
        ThemeManager.Current.ChangeThemeBaseColor(App.Current, base_color_scheme);
    }

    partial void OnSelectedApplicationColorChanged(Theme value)
    {
        ThemeManager.Current.ChangeThemeColorScheme(App.Current, SelectedApplicationColor.ColorScheme);
    }
}