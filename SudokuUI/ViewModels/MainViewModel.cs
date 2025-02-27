using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Archive.DancingLinks;
using Core.Model;
using Core.Strategies;
using MahApps.Metro.Controls.Dialogs;
using SudokuUI.Controllers;
using System.Text;

namespace SudokuUI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly Puzzle puzzle = new("4......38.32.941...953..24.37.6.9..4.29..16.36.47.3.9.957..83....39..4..24..3.7.9");
    private SelectionController selection_controller = new();

    [ObservableProperty]
    private GridViewModel _grid;

    [ObservableProperty]
    private SelectionViewModel _selections;

    public MainViewModel()
    {
        BasicEliminationStrategy.ExecuteAndApply(puzzle.Grid);
        _grid = new GridViewModel(puzzle.Grid, selection_controller);
        _selections = new SelectionViewModel(puzzle.Grid, selection_controller);
    }

    [RelayCommand]
    private async Task ShowSolutionCount()
    {
        var results = DancingLinksSolver.Solve(puzzle);
        var sb = new StringBuilder();

        sb.AppendLine($"The puzzle has {results.Count} solutions");
        sb.AppendLine($"Execution Time: {puzzle.Stats.ElapsedTime} ms");

        await DialogCoordinator.Instance.ShowMessageAsync(this, "Solution Count", sb.ToString());
    }

    [RelayCommand]
    private void NumberKeyPressed(string number)
    {
        if (int.TryParse(number, out int digit))
        {
            selection_controller.DigitSelected = digit;
        }
    }

    [RelayCommand]
    private void ClearSelection()
    {
        selection_controller.ClearDigitSelection();
    }

    [RelayCommand]
    private void ToggleInputMode()
    {
        selection_controller.ToggleInputMode();
    }
}
