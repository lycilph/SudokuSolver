using ControlzEx.Theming;
using MahApps.Metro.Controls;

namespace SudokuUI.Views;

public partial class MainWindow : MetroWindow
{
    private bool is_light = true;
    private bool is_steel = true;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void ToggleThemeButtonClick(object sender, System.Windows.RoutedEventArgs e)
    {
        if (is_light)
            ThemeManager.Current.ChangeThemeBaseColor(this, "Dark");
        else
            ThemeManager.Current.ChangeThemeBaseColor(this, "Light");

        is_light = !is_light;
    }

    private void ToggleColorButtonClick(object sender, System.Windows.RoutedEventArgs e)
    {
        if (is_steel)
            ThemeManager.Current.ChangeThemeColorScheme(this, "Green");
        else
            ThemeManager.Current.ChangeThemeColorScheme(this, "Steel");

        is_steel = !is_steel;
    }
}