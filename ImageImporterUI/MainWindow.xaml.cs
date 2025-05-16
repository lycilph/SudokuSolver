using System.Windows;
using CommunityToolkit.Mvvm.Messaging;

namespace ImageImporterUI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send("Loaded");
    }
}