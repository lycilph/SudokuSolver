using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Core;
using Core.Commands;
using Core.Extensions;
using Core.Models;
using NLog;
using ObservableCollections;
using SudokuUI.Infrastructure;
using SudokuUI.Messages;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace SudokuUI.Services;

public class PuzzleService : ObservableRecipient, IRecipient<ResetMessage>, IRecipient<MainWindowLoadedMessage>
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private readonly UndoRedoService undo_service;
    private readonly Stopwatch stopwatch;

    private readonly string empty_source = ".................................................................................";
    private string source = string.Empty;

    public Grid Grid { get; private set; } = new Grid();

    public event EventHandler<GridValuesChangedEventArgs> ValuesChanged = null!;
    public event EventHandler GridChanged = null!;
    public event EventHandler PuzzleSolved = null!;

    public PuzzleService(UndoRedoService undo_service)
    {
        this.undo_service = undo_service;
        stopwatch = Stopwatch.StartNew();

        foreach (var cell in Grid.Cells)
        {
            cell.PropertyChanged += CellChanged;
            cell.Candidates.CollectionChanged += CandidatesChanged;
        }

        IsActive = true;
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

            if (Grid.IsSolved())
            {
                logger.Info("Puzzle solved");
                stopwatch.Stop();
                PuzzleSolved?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public TimeSpan GetElapsedTime() => stopwatch.Elapsed;

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
            source = task.Result;
            WeakReferenceMessenger.Default.Send(new ResetMessage());
        }, TaskScheduler.FromCurrentSynchronizationContext());

        return task;
    }

    public void Clear()
    {
        logger.Info("Clearing the grid");
        source = empty_source;
        WeakReferenceMessenger.Default.Send(new ResetMessage());
    }

    public void Import(string puzzle)
    {
        logger.Info($"Importing the puzzle: {puzzle}");

        source = puzzle;
        WeakReferenceMessenger.Default.Send(new ResetMessage());
    }

    public string Export()
    {
        var str = Grid.ToSimpleString();

        logger.Info($"Importing the puzzle: {str}");

        return str;
    }

    public void SetCellValue(Cell cell, int value)
    {
        logger.Info($"Setting cell {cell.Index} to {value}");

        var aggregate = new AggregateCommand();

        aggregate.Add(new SetCellValueCommand(cell, value));

        var peers = cell.Peers.Where(c => c.IsEmpty && c.Count() > 0);
        aggregate.Add(new EliminateCandidatesCommand(peers));

        undo_service.Execute(aggregate);
    }

    public void ClearCell(Cell cell)
    {
        logger.Info($"Clearing cell {cell.Index}");

        undo_service.Execute(new ClearCellCommand(cell));
    }

    public void ToggleCellCandidate(Cell cell, int value)
    {
        logger.Info($"Toggling candidate {value} for cell {cell.Index}");

        undo_service.Execute(new ToggleCellCandidateCommand(cell, value));
    }

    public void FillCandidates()
    {
        logger.Info("Filling in candidates");

        var cells = Grid.EmptyCells().Where(c => c.Count() == 0).ToList();
        if (cells.Count > 0)
        {
            var aggregate = new AggregateCommand();

            aggregate.Add(new FillCandidatesCommand(cells));
            aggregate.Add(new EliminateCandidatesCommand(cells));

            undo_service.Execute(aggregate);
        }
    }

    public void ClearCandidates()
    { 
        logger.Info("Clearing candidates");

        var cells = Grid.EmptyCells().Where(c => c.Count() > 0);
        undo_service.Execute(new ClearCandidatesCommand(cells));
    }

    public void Receive(ResetMessage message)
    {
        logger.Info("Received a reset message");
        Grid.Load(source);
        stopwatch.Restart();
    }

    public void Receive(MainWindowLoadedMessage message)
    {
        logger.Info("Received the main window loaded message");

        var assembly = Assembly.GetExecutingAssembly();
        var mode = BuildModeDetector.GetBuildMode(assembly);

        if (mode == BuildModeDetector.BuildMode.Debug)
            New();
    }
}
