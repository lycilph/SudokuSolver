using CommunityToolkit.Mvvm.ComponentModel;
using ControlzEx.Theming;
using Core.Extensions;
using System.Collections.ObjectModel;

namespace SudokuUI.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isOpen = false;

    [ObservableProperty]
    private bool _isLight = true;

    [ObservableProperty]
    private Theme _selectedApplicationColor;

    public ObservableCollection<Theme> ApplicationColors { get; set; }

    public SettingsViewModel()
    {
        ApplicationColors = ThemeManager.Current.Themes.Where(t => t.BaseColorScheme == "Light").ToObservableCollection();

        var current_base_color_scheme = ThemeManager.Current.DetectTheme()?.BaseColorScheme ?? "Light";
        IsLight = current_base_color_scheme == "Light";

        var current_color_scheme = ThemeManager.Current.DetectTheme()?.ColorScheme ?? "Steel";
        SelectedApplicationColor = ApplicationColors.First(t => t.ColorScheme == current_color_scheme);
    }

    partial void OnIsLightChanged(bool value)
    {
        var base_color_scheme = IsLight ? "Light" : "Dark";
        ThemeManager.Current.ChangeThemeBaseColor(App.Current, base_color_scheme);
    }

    partial void OnSelectedApplicationColorChanged(Theme value)
    {
        ThemeManager.Current.ChangeThemeColorScheme(App.Current, SelectedApplicationColor.ColorScheme);
    }
}