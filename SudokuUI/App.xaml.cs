using SudokuUI.ViewModels;
using System.Windows;

namespace SudokuUI;

public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var win = new MainWindow
        {
            DataContext = new MainViewModel()
        };

        win.Show();
    }
}

