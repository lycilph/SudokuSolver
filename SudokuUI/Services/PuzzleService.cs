using System.ComponentModel;
using Core;
using Core.Commands;
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

    public Task<string> New()
    {
        logger.Info("Generating a new puzzle");
        
        var task = Task.Run(() =>
        {
            (var temp, _) = Generator.Generate();
            return temp.ToSimpleString();
        });

        task.ContinueWith(task =>
        {
            logger.Info("Generated puzzle: {0}", task.Result);
            Grid.Load(task.Result);
        }, TaskScheduler.FromCurrentSynchronizationContext());

        return task;
    }

    public void Clear()
    {
        logger.Info("Clearing the grid");
        Grid.Reset();
    }

    public void FillCandidates()
    {
        logger.Info("Filling in candidates");

        var cells = Grid.EmptyCells().Where(c => c.Count() == 0).ToList();
        if (cells.Count > 0)
        {
            cells.ForEach(c => c.FillCandidates());
            var command = new EliminateCandidatesCommand(cells);
            command.Do();
        }
    }

    public void ClearCandidates()
    { 
        logger.Info("Clearing candidates");
        Grid.ClearCandidates();
    }
}
