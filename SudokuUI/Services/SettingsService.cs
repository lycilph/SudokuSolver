using ControlzEx.Theming;
using NLog;
using SudokuUI.Infrastructure;
using System.IO;
using System.Text.Json;

namespace SudokuUI.Services;

public class SettingsService
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public static readonly string Light = "Light";
    public static readonly string Dark = "Dark";
    public static readonly string DefaultColorScheme = "Steel";

    public readonly string settings_file = "settings.json";

    public List<Theme> Themes { get; private set; }

    public SettingsService()
    {
        Themes = ThemeManager.Current.Themes.Where(t => t.BaseColorScheme == Light).ToList();
    }

    public string GetBaseColorScheme() => ThemeManager.Current.DetectTheme()?.BaseColorScheme ?? Light;

    public string GetColorScheme() => ThemeManager.Current.DetectTheme()?.ColorScheme ?? DefaultColorScheme;

    public void SetBaseColorScheme(bool light)
    {
        var base_color_scheme = light ? Light : Dark;
        ThemeManager.Current.ChangeThemeBaseColor(App.Current, base_color_scheme);
        Save();
    }

    public void SetColorScheme(string color_scheme)
    {
        ThemeManager.Current.ChangeThemeColorScheme(App.Current, color_scheme);
        Save();
    }

    public void Load()
    {
        try
        {
            if (File.Exists(settings_file))
            {
                string json = File.ReadAllText(settings_file);
                var settings = JsonSerializer.Deserialize<Settings>(json);

                if (settings != null)
                {
                    // Apply the saved theme
                    ThemeManager.Current.ChangeTheme(App.Current, settings.BaseColorScheme, settings.ColorScheme);
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error($"Error loading theme settings: {ex.Message}");
        }
    }

    public void Save()
    {
        var settings = new Settings
        {
            BaseColorScheme = GetBaseColorScheme(),
            ColorScheme = GetColorScheme(),
        };

        try
        {
            string json = JsonSerializer.Serialize(settings);
            File.WriteAllText(settings_file, json);
        }
        catch (Exception ex)
        {
            logger.Error($"Error saving theme settings: {ex.Message}");
        }
    }
}
