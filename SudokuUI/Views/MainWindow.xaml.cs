using ControlzEx.Theming;
using MahApps.Metro.Controls;

namespace SudokuUI.Views;

public partial class MainWindow : MetroWindow
{
    private bool is_light = true;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        if (is_light)
            ThemeManager.Current.ChangeTheme(this, "Dark.Green");

        else
            ThemeManager.Current.ChangeTheme(this, "Light.Green");

        is_light = !is_light;
    }
}