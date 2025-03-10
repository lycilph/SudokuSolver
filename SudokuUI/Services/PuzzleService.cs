using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Infrastructure;
using Core.Model;
using Core.Model.Actions;
using Core.Strategy;
using NLog;
using System.ComponentModel;

namespace SudokuUI.Services;

public partial class PuzzleService : ObservableObject
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public Grid Grid { get; private set; }

    public event EventHandler GridValuesChanged = null!;

    private Stack<IPuzzleAction> undo_stack = new();
    private Stack<IPuzzleAction> redo_stack = new();

    public PuzzleService()
    {
#if DEBUG
        Grid = new Grid("4......38.32.941...953..24.37.6.9..4.29..16.36.47.3.9.957..83....39..4..24..3.7.9");
        var action = BasicEliminationStrategy.ExecuteAndApply(Grid);
        AddPuzzleAction(action!);
#else
        Grid = new Grid();
        Grid.EmptyCells().ForEach(cell => cell.Candidates.Clear());
#endif

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
    
    private bool CanUndo() => undo_stack.Count > 0;

    [RelayCommand(CanExecute = nameof(CanUndo))]
    private void Undo()
    {
        var action = undo_stack.Pop();
        redo_stack.Push(action);

        action.Undo();

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

        UndoCommand.NotifyCanExecuteChanged();
        RedoCommand.NotifyCanExecuteChanged();
    }
}
