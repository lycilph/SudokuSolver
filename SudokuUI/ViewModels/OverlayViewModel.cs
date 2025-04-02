using CommunityToolkit.Mvvm.ComponentModel;
using Core.Commands;
using Core.Engine;
using SudokuUI.Infrastructure;

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
    private HintsViewModel hintsVM;

    [ObservableProperty]
    private System.Windows.Input.ICommand escapeCommand = null!;

    public OverlayViewModel(NewGameViewModel newGameVM, VictoryViewModel victoryVM, HintsViewModel hintsVM)
    {
        NewGameVM = newGameVM;
        VictoryVM = victoryVM;
        HintsVM = hintsVM;
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

    public bool CanHide() => !ShowSpinner && !VictoryVM.IsOpen && !HintsVM.IsOpen;

    public OverlayScope GetScope(bool show_spinner = false) => new(this, show_spinner);

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

    public Task ShowHint(BaseCommand? cmd)
    {
        Show();
        HintsVM.Show(cmd);

        return HintsVM.Task
            .ContinueWith(t =>
            {
                HintsVM.Hide();
                Hide();
                return t;
            }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    public void CancelHints()
    {
        HintsVM.Cancel();
    }
}