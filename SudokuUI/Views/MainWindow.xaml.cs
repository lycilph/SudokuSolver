using System.ComponentModel;
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
        Closing += MainWindowClosing;
    }

    private void MainWindowClosing(object? sender, CancelEventArgs e)
    {
        Closing -= MainWindowClosing;
        WeakReferenceMessenger.Default.Send(new MainWindowClosingMessage());
    }

    private void MainWindowLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= MainWindowLoaded;
        WeakReferenceMessenger.Default.Send(new MainWindowLoadedMessage());
    }
}