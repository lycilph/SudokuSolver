using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Core.DancingLinks;
using Core.Extensions;
using Core.Models;
using NLog;
using SudokuUI.Messages;
using SudokuUI.ViewModels;
using SudokuUI.Visualizers.Misc;

namespace SudokuUI.Services;

public partial class HighlightService : ObservableObject
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private readonly PuzzleService puzzle_service;
    private readonly SelectionService selection_service;
    private readonly GridViewModel grid_vm;

    [ObservableProperty]
    private ObservableCollection<LinkVisualizer> links = [];

    public HighlightService(PuzzleService puzzle_service, SelectionService selection_service, GridViewModel grid_vm)
    {
        this.puzzle_service = puzzle_service;
        this.selection_service = selection_service;
        this.grid_vm = grid_vm;

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
        // Clear in case some cells are marked as errors
        Clear();

        var cell_vms = grid_vm.Boxes.SelectMany(b => b.Cells);
        foreach (var vm in cell_vms)
            vm.Highlight = vm.WrappedObject.Value == number;

        var candidate_vms = grid_vm.Boxes.SelectMany(b => b.Cells).SelectMany(c => c.Candidates);
        foreach (var vm in candidate_vms)
            vm.Highlight = vm.Value == number && vm.CellHasCandidate();
    }

    public void ValidateGrid()
    {
        var copy = new Grid().Load(puzzle_service.Source, true);
        (var solutions, _) = DancingLinksSolver.Solve(copy, true, 5);

        if (solutions.Count != 1)
        {
            WeakReferenceMessenger.Default.Send(new ShowNotificationMessage("More than 1 solution!"));
            return;
        }
        else
            copy = solutions.Single();

        var cell_vms = grid_vm.Boxes.SelectMany(b => b.Cells).ToList();
        var errors_found = 0;
        foreach (var cell in puzzle_service.Grid.Cells)
        {
            if (cell.IsFilled && cell.Value != copy.Cells[cell.Index].Value)
            {
                // Mark this as wrong
                var vm = cell_vms.Single(vm => vm.WrappedObject.Index == cell.Index);
                vm.MarkError();
                errors_found++;
            }
        }

        if (errors_found == 0)
            WeakReferenceMessenger.Default.Send(new ShowNotificationMessage("No errors found"));
    }

    public void Add(Cell start, Cell end, Brush color, LinkVisualizer.LineType line_type = LinkVisualizer.LineType.Solid)
    {
        Links.Add(new LinkVisualizer(start, end, color, line_type));
    }

    public void Add(Link link, Brush color, LinkVisualizer.LineType line_type = LinkVisualizer.LineType.Solid)
    {
        Links.Add(new LinkVisualizer(link.Start, link.End, color, line_type));
    }

    public void Clear()
    {
        grid_vm.Boxes.SelectMany(b => b.Cells).ForEach(c => c.ResetVisuals());
        grid_vm.Boxes.SelectMany(b => b.Cells).SelectMany(c => c.Candidates).ForEach(c => c.ResetVisuals());

        Links.Clear();
    }
}
