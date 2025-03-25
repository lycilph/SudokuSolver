using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core;
using Core.Commands;
using Core.Extensions;
using ObservableCollections;
using SudokuUI.Messages;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class DebugViewModel : ObservableObject
{
    private readonly PuzzleService puzzle_service;
    private readonly UndoRedoService undo_service;

    public NotifyCollectionChangedSynchronizedViewList<ICommand> UndoStack { get; private set; }
    public NotifyCollectionChangedSynchronizedViewList<ICommand> RedoStack { get; private set; }

    public ObservableCollection<StrategyViewModel> Strategies { get; private set; }

    [ObservableProperty]
    private ICommand? selected;

    public DebugViewModel(PuzzleService puzzle_service, UndoRedoService undo_service, SolverService solver_service)
    {
        this.puzzle_service = puzzle_service;
        this.undo_service = undo_service;

        UndoStack = undo_service.UndoStack.ToNotifyCollectionChanged();
        RedoStack = undo_service.RedoStack.ToNotifyCollectionChanged();

        Strategies = solver_service.Strategies.Select(s => new StrategyViewModel(s)).ToObservableCollection();
    }

    [RelayCommand]
    private void Toggle()
    {
        if (Strategies.All(s => s.Selected))
            Strategies.ForEach(s => s.Selected = false);
        else
            Strategies.ForEach(s => s.Selected = true); 
    }

    [RelayCommand]
    private void Next()
    {
        var strategies = Strategies.Where(s => s.Selected).Select(s => s.WrappedObject).ToArray();
        var command = Solver.Step(puzzle_service.Grid, strategies);

        if (command != null)
            undo_service.Execute(command);
        else
            WeakReferenceMessenger.Default.Send(new ShowNotificationMessage("No more hints found"));
    }
}
