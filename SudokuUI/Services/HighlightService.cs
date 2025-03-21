using System.ComponentModel;
using NLog;

namespace SudokuUI.Services;

public class HighlightService
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private readonly PuzzleService puzzle_service;
    private readonly SelectionService selection_service;

    public HighlightService(PuzzleService puzzle_service, SelectionService selection_service)
    {
        this.puzzle_service = puzzle_service;
        this.selection_service = selection_service;

        selection_service.PropertyChanged += SelectionChanged;
        puzzle_service.GridChanged += GridChanged;
    }

    private void GridChanged(object? sender, EventArgs e)
    {
        HighlightNumber(selection_service.Digit);
    }

    private void SelectionChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectionService.Digit))
        {
            logger.Debug("SelectionService.Digit changed (highlighting {0})", selection_service.Digit);
            HighlightNumber(selection_service.Digit);
        }
    }

    public void HighlightNumber(int number)
    {
        logger.Debug("Highlighting {0}", selection_service.Digit);

        var grid_vm = puzzle_service.GridVM;

        var cell_vms = grid_vm.Boxes.SelectMany(b => b.Cells);
        foreach (var vm in cell_vms)
            vm.Highlight = vm.WrappedObject.Value == number;

        var candidate_vms = grid_vm.Boxes.SelectMany(b => b.Cells).SelectMany(c => c.Candidates);
        foreach (var vm in candidate_vms)
            vm.Highlight = vm.Value == number && vm.CellHasCandidate();
    }
}
