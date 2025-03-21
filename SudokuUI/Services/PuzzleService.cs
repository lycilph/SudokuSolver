using System.ComponentModel;
using Core;
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
        GridVM = new GridViewModel(Grid);

        // https://stackoverflow.com/questions/1611410/how-to-check-if-a-app-is-in-debug-or-release

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

    public void NewPuzzle()
    {
        logger.Info("Generating a new puzzle");
        (var temp, _) = Generator.Generate();
        var str = temp.ToSimpleString();

        Grid.Load(str);
    }
}
