using CommunityToolkit.Mvvm.ComponentModel;
using Core.Engine;
using SudokuUI.Infrastructure;
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
    private VictoryViewModel victoryVM;

    [ObservableProperty]
    private ICommand escapeCommand = null!;

    public OverlayViewModel(NewGameViewModel newGameVM, VictoryViewModel victoryVM)
    {
        NewGameVM = newGameVM;
        VictoryVM = victoryVM;
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

    public bool CanHide() => !ShowSpinner && !VictoryVM.IsOpen;

    public Task<Difficulty?> ShowNewGame()
    {
        Show();
        NewGameVM.Show();

        return NewGameVM.Task
            .ContinueWith(t => 
            {
                NewGameVM.Hide();
                Hide();
                return t.Result;
            });
    }

    public Task<VictoryResult> ShowVictory(TimeSpan time)
    {
        Show();
        VictoryVM.Show(time);

        return VictoryVM.Task
            .ContinueWith(t => 
            {
                VictoryVM.Hide();
                Hide();
                return t.Result;
            });
    }
}
