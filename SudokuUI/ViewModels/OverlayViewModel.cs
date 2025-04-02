using CommunityToolkit.Mvvm.ComponentModel;
using Core.Commands;
using Core.Engine;
using SudokuUI.Infrastructure;

namespace SudokuUI.ViewModels;

public partial class OverlayViewModel : ObservableObject
{
    private TaskCompletionSource open_animation_completion_source = new();
    private TaskCompletionSource close_animation_completion_source = new();

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

    public void OnOpenAnimationCompleted() => open_animation_completion_source.SetResult();
    public void OnCloseAnimationCompleted() => close_animation_completion_source.SetResult();

    public Task Show(bool show_spinner = false)
    {
        open_animation_completion_source = new();
        close_animation_completion_source = new();

        IsOpen = true;
        ShowSpinner = show_spinner;

        return open_animation_completion_source.Task;
    }

    public void Hide()
    {
        if (NewGameVM.IsActive)
            NewGameVM.Cancel();

        IsOpen = false;
        ShowSpinner = false;
    }

    public bool CanHide() => !ShowSpinner && !VictoryVM.IsActive && !HintsVM.IsActive;

    public OverlayScope GetWaitingSpinnerScope(bool show_spinner = false) => new(this, show_spinner);

    public void AddVictoryStatistics(Statistics stats) => VictoryVM.AddStatistics(stats);

    public async Task<Difficulty?> ShowNewGame()
    {
        Show();
        var difficulty = await NewGameVM.Activate();
        
        Hide();
        await close_animation_completion_source.Task;

        return difficulty;
    }

    public async Task<VictoryResult> ShowVictory(TimeSpan time)
    {
        Show();
        var result = await VictoryVM.Activate(time);

        Hide();
        await close_animation_completion_source.Task;

        return result;
    }

    public async Task ShowHint(BaseCommand? cmd)
    {
        Show();
        await HintsVM.Activate(cmd);

        Hide();
        await close_animation_completion_source.Task;
    }

    public async Task CancelHints()
    {
        HintsVM.Cancel();
        await close_animation_completion_source.Task;
    }
}