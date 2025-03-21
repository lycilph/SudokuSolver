using CommunityToolkit.Mvvm.ComponentModel;
using ControlzEx.Theming;

namespace SudokuUI.Services;

public partial class SettingsService : ObservableObject
{
    public static readonly string Light = "Light";
    public static readonly string Dark = "Dark";
    public static readonly string DefaultColorScheme = "Steel";

    [ObservableProperty]
    private bool isShown = false;

    public List<Theme> Themes { get; private set; }

    public SettingsService()
    {
        Themes = ThemeManager.Current.Themes.Where(t => t.BaseColorScheme == Light).ToList();
    }

    public string GetBaseColorScheme() => ThemeManager.Current.DetectTheme()?.BaseColorScheme ?? Light;

    public string GetColorScheme() => ThemeManager.Current.DetectTheme()?.ColorScheme ?? DefaultColorScheme;

    public void Show() => IsShown = true;
    public void Hide() => IsShown = false;
}
