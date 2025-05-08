using System.Windows;
using ImageImportUI.MVVM;

namespace ImageImportUI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}