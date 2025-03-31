using CommunityToolkit.Mvvm.ComponentModel;
using Core.Engine;
using System.Windows.Input;

namespace SudokuUI.ViewModels;

public partial class OverlayViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isOpen = false;

    [ObservableProperty]
    private bool showSpinner = false;

    [ObservableProperty]
    private NewGameViewModel newGameVM;

    [ObservableProperty]
    private ICommand escapeCommand = null!;

    public OverlayViewModel(NewGameViewModel newGameVM)
    {
        NewGameVM = newGameVM;
    }

    public void Show(bool show_spinner = false)
    {
        IsOpen = true;
        ShowSpinner = show_spinner;
    }

    public void Hide()
    {
        if (NewGameVM.IsOpen)
            NewGameVM.Cancel();

        IsOpen = false;
        ShowSpinner = false;
    }

    public Task<Difficulty?> ShowNewGame()
    {
        Show();

        NewGameVM.Show();
        NewGameVM.Task.ContinueWith(_ =>
        {
            NewGameVM.Hide();
            Hide();
        });

        return NewGameVM.Task;
    }
}
