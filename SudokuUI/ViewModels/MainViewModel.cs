using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.DancingLinks;
using Core.Extensions;
using MahApps.Metro.Controls.Dialogs;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly PuzzleService puzzle_service;
    private readonly SelectionService selection_service;
    private readonly SettingsService settings_service;
    private readonly DebugService debug_service;

    [ObservableProperty]
    private bool isKeyboardDisabled = false;

    [ObservableProperty]
    private GridViewModel gridVM;

    [ObservableProperty]
    private DigitSelectionViewModel digitSelectionVM;

    [ObservableProperty]
    private SettingsViewModel settingsVM;

    [ObservableProperty]
    private SettingsOverlayViewModel settingsOverlayVM;

    public MainViewModel(PuzzleService puzzle_service,
                         SelectionService selection_service,
                         SettingsService settings_service,
                         DebugService debug_service,
                         DigitSelectionViewModel digitSelectionVM,
                         SettingsViewModel settingsVM,
                         SettingsOverlayViewModel settingsOverlayVM)
    {
        this.puzzle_service = puzzle_service;
        this.selection_service = selection_service;
        this.settings_service = settings_service;
        this.debug_service = debug_service;

        GridVM = puzzle_service.GridVM;
        DigitSelectionVM = digitSelectionVM;
        SettingsVM = settingsVM;
        SettingsOverlayVM = settingsOverlayVM;

        settings_service.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SettingsService.IsShown))
                IsKeyboardDisabled = settings_service.IsShown;
        };
    }

    [RelayCommand]
    private async Task ShowSolutionCount()
    {
        var copy = puzzle_service.Grid.Copy();
        copy.FillCandidates();

        (var solutions, var stats) = DancingLinksSolver.Solve(copy, true);

        var sb = new StringBuilder();
        sb.AppendLine($"The puzzle has {solutions.Count} solutions");
        sb.AppendLine($"Execution Time: {stats.ElapsedTime} ms");
        await DialogCoordinator.Instance.ShowMessageAsync(this, "Solution Count", sb.ToString());
    }

    [RelayCommand]
    private void NumberKeyPressed(string number)
    {
        if (int.TryParse(number, out int digit))
            selection_service.Digit = digit;
    }

    [RelayCommand]
    private void Escape()
    {
        if (settings_service.IsShown)
            settings_service.Hide();
        else
            selection_service.Clear();
    }

    [RelayCommand]
    private void NextDigit()
    {
        selection_service.Next();
    }

    [RelayCommand]
    private void PreviousDigit()
    {
        selection_service.Previous();
    }

    [RelayCommand]
    private void ToggleInputMode()
    {
        selection_service.ToggleInputMode();
    }

    [RelayCommand]
    private void ShowSettings()
    {
        settings_service.Show();
    }

    [RelayCommand]
    private void HideSettings()
    {
        settings_service.Hide();
    }

    [RelayCommand]
    private void ShowDebugWindow()
    {
        debug_service.ShowDebugWindow();
    }

    [RelayCommand]
    private void Test()
    {
        ShowDebugWindow();
    }
}