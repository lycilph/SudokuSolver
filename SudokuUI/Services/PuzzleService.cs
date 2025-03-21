using System.ComponentModel;
using Core.Extensions;
using Core.Models;
using NLog;
using ObservableCollections;
using SudokuUI.Infrastructure;
using SudokuUI.ViewModels;

namespace SudokuUI.Services;

public class PuzzleService
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public Grid Grid { get; private set; } = new Grid();
    public GridViewModel GridVM { get; private set; }

    public event EventHandler<GridValuesChangedEventArgs> ValuesChanged = null!;
    public event EventHandler GridChanged = null!;

    public PuzzleService()
    {
        // DEBUG
        Grid.Load(".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4", true); // These numbers should show as clues
        GridVM = new GridViewModel(Grid);

        foreach (var cell in Grid.Cells)
        {
            cell.PropertyChanged += CellChanged;
            cell.Candidates.CollectionChanged += CandidatesChanged;
        }
    }

    private void CandidatesChanged(in NotifyCollectionChangedEventArgs<int> e)
    {
        logger.Trace("Grid candidates changed");
        GridChanged?.Invoke(this, EventArgs.Empty);
    }

    private void CellChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Cell.Value))
        {
            logger.Trace("Grid values changed");
            ValuesChanged?.Invoke(this, new GridValuesChangedEventArgs(DigitCount()));
            GridChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public List<int> DigitCount()
    {
        var numbers = Enumerable.Repeat(0, 9).ToList();
        foreach (var cell in Grid.Cells.Where(c => c.IsFilled))
            numbers[cell.Value - 1]++;
        return numbers;
    }
}
