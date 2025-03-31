using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SudokuUI.ViewModels;

public partial class OverlayViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isOpen = false;

    [ObservableProperty]
    private bool showSpinner = false;

    [ObservableProperty]
    private ICommand? clickCommand = null;
}
