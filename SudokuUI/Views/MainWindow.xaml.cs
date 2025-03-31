using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using SudokuUI.Messages;

namespace SudokuUI.Views;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindowLoaded;
    }

    private void MainWindowLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= MainWindowLoaded;
        WeakReferenceMessenger.Default.Send(new MainWindowLoadedMessage());
    }
}