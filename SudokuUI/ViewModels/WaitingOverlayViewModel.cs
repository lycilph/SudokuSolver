using CommunityToolkit.Mvvm.ComponentModel;

namespace SudokuUI.ViewModels;

public partial class WaitingOverlayViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isOpen = false;

    public void Show() => IsOpen = true;
    public void Close() => IsOpen = false;
    public void Toggle() => IsOpen = !IsOpen;
}
