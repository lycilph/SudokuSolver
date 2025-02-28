using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using SudokuUI.Services;
using System.Text;

namespace SudokuUI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private PuzzleService puzzle_service;
    private SelectionService selection_service;

    [ObservableProperty]
    private GridViewModel _grid;

    [ObservableProperty]
    private SelectionViewModel _selections;

    public MainViewModel(PuzzleService puzzle_service, SelectionService selection_service)
    {
        this.puzzle_service = puzzle_service;
        this.selection_service = selection_service;

        _grid = new GridViewModel(puzzle_service.Grid);
        _selections = new SelectionViewModel(puzzle_service.Grid, selection_service);
    }

    [RelayCommand]
    private async Task ShowSolutionCount()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"The puzzle has {puzzle_service.GetSolutionCount()} solutions");
        sb.AppendLine($"Execution Time: {puzzle_service.GetStatistics().ElapsedTime} ms");
        await DialogCoordinator.Instance.ShowMessageAsync(this, "Solution Count", sb.ToString());
    }

    [RelayCommand]
    private void NumberKeyPressed(string number)
    {
        if (int.TryParse(number, out int digit))
            selection_service.Digit = digit;
    }

    [RelayCommand]
    private void ClearSelection()
    {
        selection_service.ClearDigitSelection();
    }

    [RelayCommand]
    private void ToggleInputMode()
    {
        selection_service.ToggleInputMode();
    }
}
