using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Model;
using Core.Model.Actions;
using NLog;
using System.ComponentModel;

namespace SudokuUI.Services;

public partial class PuzzleService : ObservableObject
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public string source = string.Empty;
    public Grid Grid { get; private set; }

    public event EventHandler GridValuesChanged = null!;

    private readonly Stack<IPuzzleAction> undo_stack = new();
    private readonly Stack<IPuzzleAction> redo_stack = new();

    public PuzzleService()
    {
        Grid = new Grid();
        Grid.ClearCandidates();

        foreach (var cell in Grid.Cells)
            cell.PropertyChanged += CellChanged;
    }

    private void CellChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Cell.Value))
            OnGridValuesChanged();
    }

    private void OnGridValuesChanged()
    {
        logger.Debug("Grid values changed");
        GridValuesChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ResetPuzzle()
    {
        Grid.Set(source);
        Grid.ClearCandidates();

        undo_stack.Clear();
        redo_stack.Clear();

        ResetCommand.NotifyCanExecuteChanged();
        UndoCommand.NotifyCanExecuteChanged();
        RedoCommand.NotifyCanExecuteChanged();
    }

    public void Clear()
    {
        logger.Info("Clearing puzzle");

        Grid.ResetCells();
        Grid.ClearCandidates();

        undo_stack.Clear();
        redo_stack.Clear();

        ResetCommand.NotifyCanExecuteChanged();
        UndoCommand.NotifyCanExecuteChanged();
        RedoCommand.NotifyCanExecuteChanged();
    }

    public void Import(string puzzle)
    {
        logger.Info($"Importing the puzzle: {puzzle}");

        source = puzzle;
        ResetPuzzle();
    }

    public string Export()
    {
        var str = Grid.ToSimpleString();

        logger.Info($"Importing the puzzle: {str}");

        return str;
    }

    private bool CanReset() => CanUndo() || CanRedo();

    [RelayCommand(CanExecute = nameof(CanReset))]
    public void Reset()
    {
        logger.Info("Resetting the puzzle");

        ResetPuzzle();
    }

    private bool CanUndo() => undo_stack.Count > 0;

    [RelayCommand(CanExecute = nameof(CanUndo))]
    private void Undo()
    {
        var action = undo_stack.Pop();
        redo_stack.Push(action);

        action.Undo();

        ResetCommand.NotifyCanExecuteChanged(); 
        UndoCommand.NotifyCanExecuteChanged();
        RedoCommand.NotifyCanExecuteChanged();
    }
    
    private bool CanRedo() => redo_stack.Count > 0;

    [RelayCommand(CanExecute = nameof(CanRedo))]
    private void Redo()
    {
        var action = redo_stack.Pop();
        undo_stack.Push(action);

        action.Do();

        ResetCommand.NotifyCanExecuteChanged(); 
        UndoCommand.NotifyCanExecuteChanged();
        RedoCommand.NotifyCanExecuteChanged();
    }

    public IEnumerable<int> CountDigits()
    {
        return Grid.PossibleValues.Select(i => Grid.FilledCells().Count(c => c.Value == i));
    }

    public void SetValue(Cell cell, int value)
    {
        AddPuzzleAction(new SetValuePuzzleAction(cell, value));
    }

    public void ToggleCandidate(Cell cell, int value)
    {
        AddPuzzleAction(new ToggleCandidatePuzzleAction(cell, value));
    }

    public void ClearCell(Cell cell)
    {
        AddPuzzleAction(new ClearCellPuzzleAction(cell));
    }

    private void AddPuzzleAction(IPuzzleAction action)
    {
        action.Do();

        undo_stack.Push(action);
        redo_stack.Clear();

        ResetCommand.NotifyCanExecuteChanged();
        UndoCommand.NotifyCanExecuteChanged();
        RedoCommand.NotifyCanExecuteChanged();
    }
}
