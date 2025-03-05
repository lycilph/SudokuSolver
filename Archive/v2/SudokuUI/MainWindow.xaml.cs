using CommunityToolkit.Mvvm.Messaging;
using MahApps.Metro.Controls;
using SudokuUI.Messages;

namespace SudokuUI;

public partial class MainWindow : MetroWindow
{
    public MainWindow() 
    {
        InitializeComponent();
    }

    private void WindowContentRendered(object sender, EventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new WindowShownMessage());
    }
}