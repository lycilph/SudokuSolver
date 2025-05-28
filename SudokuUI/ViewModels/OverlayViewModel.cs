using CommunityToolkit.Mvvm.ComponentModel;
using Core.Commands;
using Core.Engine;
using SudokuUI.Infrastructure;

namespace SudokuUI.ViewModels;

public partial class OverlayViewModel : ObservableObject
{
    private readonly double standard_opacity = 0.75;
    private readonly double hints_view_opacity = 0.1;

    private TaskCompletionSource open_animation_completion_source = new();
    private TaskCompletionSource close_animation_completion_source = new();

    [ObservableProperty]
    private bool isOpen = false;

    [ObservableProperty]
    private bool showSpinner = false;

    [ObservableProperty]
    private string message = string.Empty;

    [ObservableProperty]
    private double overlayOpacity = 0.5;

    [ObservableProperty]
    private NewGameViewModel newGameVM;
    
    [ObservableProperty]
    private VictoryViewModel victoryVM;

    [ObservableProperty]
    private HintsViewModel hintsVM;

    [ObservableProperty]
    private SettingsViewModel settingsVM;

    [ObservableProperty]
    private System.Windows.Input.ICommand escapeCommand = null!;

    public OverlayViewModel(NewGameViewModel newGameVM,
                            VictoryViewModel victoryVM,
                            HintsViewModel hintsVM,
                            SettingsViewModel settingsVM)
    {
        NewGameVM = newGameVM;
        VictoryVM = victoryVM;
        HintsVM = hintsVM;
        SettingsVM = settingsVM;
    }

    public void OnOpenAnimationCompleted() => open_animation_completion_source.SetResult();
    public void OnCloseAnimationCompleted() => close_animation_completion_source.SetResult();

    public Task Show(bool show_spinner = false, string message = "")
    {
        open_animation_completion_source = new();
        close_animation_completion_source = new();

        IsOpen = true;
        ShowSpinner = show_spinner;
        Message = message;

        return open_animation_completion_source.Task;
    }

    public void Hide()
    {
        if (NewGameVM.IsActive)
            NewGameVM.Cancel();

        if (HintsVM.IsActive)
            HintsVM.Cancel();

        if (SettingsVM.IsActive)
            SettingsVM.Hide();

        IsOpen = false;
        ShowSpinner = false;
    }

    public bool CanHide() => !ShowSpinner && !VictoryVM.IsActive;
    public bool CanToggleSettings() => !ShowSpinner && !NewGameVM.IsActive && !VictoryVM.IsActive && !HintsVM.IsActive;

    public OverlayScope GetWaitingSpinnerScope(bool show_spinner = false, string message = "") => new(this, show_spinner, message);

    public void AddVictoryStatistics(Statistics stats) => VictoryVM.AddStatistics(stats);

    public async Task<Difficulty?> ShowNewGame()
    {
        _ = Show();
        var difficulty = await NewGameVM.Activate();
        
        Hide();
        await close_animation_completion_source.Task;

        return difficulty;
    }

    public async Task<VictoryResult> ShowVictory(string puzzle_source, TimeSpan time)
    {
        // If this is triggered while the user is in the hints view, cancel it here
        if (HintsVM.IsActive)
        {
            HintsVM.Cancel();
            await close_animation_completion_source.Task;
        }

        _ = Show();
        var result = await VictoryVM.Activate(puzzle_source, time);

        Hide();
        await close_animation_completion_source.Task;

        return result;
    }

    public async Task ShowHint(BaseCommand? cmd)
    {
        OverlayOpacity = hints_view_opacity;

        _ = Show();
        await HintsVM.Activate(cmd);

        Hide();
        await close_animation_completion_source.Task;

        OverlayOpacity = standard_opacity;
    }

    public void ShowSettings()
    {
        Show();
        SettingsVM.Show();
    }
}