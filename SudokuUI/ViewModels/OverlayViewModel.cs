using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Engine;

namespace SudokuUI.ViewModels;

public partial class OverlayViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isOpen = false;

    [ObservableProperty]
    private bool showSpinner = false;

    [ObservableProperty]
    private ICommand? clickCommand = null;

    [ObservableProperty]
    private NewGameViewModel newGameVM;

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
