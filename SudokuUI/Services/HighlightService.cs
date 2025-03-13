using NLog;
using SudokuUI.ViewModels;
using System.ComponentModel;

namespace SudokuUI.Services;

public class HighlightService
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private readonly GridViewModel grid;
    private readonly SelectionService selection_service;
    private readonly PuzzleService puzzle_service;

    public HighlightService(GridViewModel grid, SelectionService selection_service, PuzzleService puzzle_service)
    {
        this.grid = grid;
        this.selection_service = selection_service;
        this.puzzle_service = puzzle_service;

        selection_service.PropertyChanged += SelectionServicePropertyChanged;

        puzzle_service.GridValuesChanged += GridChanged;
        puzzle_service.GridCandidatesChanged += GridChanged;
    }

    private void GridChanged(object? sender, EventArgs e)
    {
        HighlightNumber(selection_service.Digit);
    }

    private void SelectionServicePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectionService.Digit))
        {
            HighlightNumber(selection_service.Digit);
        }
    }

    public void HighlightNumber(int number)
    {
        logger.Debug($"Highlighting number {number}");

        var cell_vms = grid.Boxes.SelectMany(b => b.Cells);
        foreach (var vm in cell_vms)
            vm.Highlight = vm.Cell.Value == number;

        var candidate_vms = grid.Boxes.SelectMany(b => b.Cells).SelectMany(c => c.Candidates);
        foreach (var vm in candidate_vms)
            vm.Highlight = vm.Value == number && vm.CellHasCandidate();
    }

    public void HighlightCell(int index)
    {
        logger.Debug($"Highlighting cell {index}");
    }
}
