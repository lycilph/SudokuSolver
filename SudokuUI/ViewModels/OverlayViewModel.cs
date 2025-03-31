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

    public Task ShowVictory()
    {
        Show();

        var task = Task.Delay(1000);
        task.ContinueWith(_ =>
        {
            VictoryVM.Hide();
            Hide();
        });
        
        return task;
    }
}
