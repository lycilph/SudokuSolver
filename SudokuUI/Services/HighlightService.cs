using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Extensions;
using Core.Models;
using NLog;
using SudokuUI.ViewModels;
using SudokuUI.Visualizers.Misc;

namespace SudokuUI.Services;

public partial class HighlightService : ObservableObject
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private readonly SelectionService selection_service;
    private readonly GridViewModel grid_vm;

    [ObservableProperty]
    private ObservableCollection<LinkVisualizer> links = [];

    public HighlightService(PuzzleService puzzle_service, SelectionService selection_service, GridViewModel grid_vm)
    {
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
        var cell_vms = grid_vm.Boxes.SelectMany(b => b.Cells);
        foreach (var vm in cell_vms)
            vm.Highlight = vm.WrappedObject.Value == number;

        var candidate_vms = grid_vm.Boxes.SelectMany(b => b.Cells).SelectMany(c => c.Candidates);
        foreach (var vm in candidate_vms)
            vm.Highlight = vm.Value == number && vm.CellHasCandidate();
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
