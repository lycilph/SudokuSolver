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

    [ObservableProperty]
    private int activeExpander = 0;

    [ObservableProperty]
    private Tuple<string, string> selectedPuzzle;

    [ObservableProperty]
    private ObservableCollection<Tuple<string, string>> puzzles = 
        [
            Tuple.Create("Naked Singles", ".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4"),
            Tuple.Create("Hidden Singles", ".28..7....16.83.7.....2.85113729.......73........463.729..7.......86.14....3..7.."),
            Tuple.Create("Pointing Candidates 1", ".179.36......8....9.....5.7.72.1.43....4.2.7..6437.25.7.1....65....3......56.172."),
            Tuple.Create("Pointing Candidates 2", "93..5....2..63..95856..2.....318.57...5.2.98..8...5......8..1595.821...4...56...8"),
            Tuple.Create("Claiming Candidates 1", ".16..78.3.9.8.....87...126..48...3..65...9.82.39...65..6.9...2..8...29369246..51."),
            Tuple.Create("Claiming Candidates 2", ".2.9437159.4...6..75.....4.5..48....2.....4534..352....42....81..5..426..9.2.85.4"),
            Tuple.Create("Naked Pairs", "4.....938.32.941...953..24.37.6.9..4529..16736.47.3.9.957..83....39..4..24..3.7.9")
        ];

    public DebugViewModel(PuzzleService puzzle_service, UndoRedoService undo_service, SolverService solver_service)
    {
        this.puzzle_service = puzzle_service;
        this.undo_service = undo_service;

        UndoStack = undo_service.UndoStack.ToNotifyCollectionChanged();
        RedoStack = undo_service.RedoStack.ToNotifyCollectionChanged();

        Strategies = solver_service.Strategies.Select(s => new StrategyViewModel(s)).ToObservableCollection();

        SelectedPuzzle = Puzzles.First();
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

    [RelayCommand]
    private void Load()
    {
        puzzle_service.Import(SelectedPuzzle.Item2);
    }
}
