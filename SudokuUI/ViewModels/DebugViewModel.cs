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
            Tuple.Create("Easy 1", "..8.......3.......9.....2..8.1..2.9..576..1.....5..7.25..8.1..3.93.....56...3..4."),
            Tuple.Create("Easy 2", ".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4"),
            Tuple.Create("Almost completed", "428.57316735126489916483257861372594257649138349518762574891623193264875682735941"),
            Tuple.Create("Naked Singles", ".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4"),
            Tuple.Create("Hidden Singles", ".28..7....16.83.7.....2.85113729.......73........463.729..7.......86.14....3..7.."),
            Tuple.Create("Pointing Candidates 1", ".179.36......8....9.....5.7.72.1.43....4.2.7..6437.25.7.1....65....3......56.172."),
            Tuple.Create("Pointing Candidates 2", "93..5....2..63..95856..2.....318.57...5.2.98..8...5......8..1595.821...4...56...8"),
            Tuple.Create("Claiming Candidates 1", ".16..78.3.9.8.....87...126..48...3..65...9.82.39...65..6.9...2..8...29369246..51."),
            Tuple.Create("Claiming Candidates 2", ".2.9437159.4...6..75.....4.5..48....2.....4534..352....42....81..5..426..9.2.85.4"),
            Tuple.Create("Naked Pairs", "4.....938.32.941...953..24.37.6.9..4529..16736.47.3.9.957..83....39..4..24..3.7.9"),
            Tuple.Create("Hidden Pairs 1", "72.4.8.3..8.....474.1.768.281.739......851......264.8.2.968.41334......8168943275"),
            Tuple.Create("Hidden Pairs 2", ".49132....81479...327685914.96.518...75.28....38.46..5853267...712894563964513..."),
            Tuple.Create("Naked Triples", ".7.4.8.29..2.....4854.2...7..83742...2.........32617......936122.....4.313.642.7."),
            Tuple.Create("Hidden Triples", ".....1.3.231.9.....65..31..6789243..1.3.5...6...1367....936.57...6.198433........"),
            Tuple.Create("Naked Quads", "....3..86....2..4..9..7852.3718562949..1423754..3976182..7.3859.392.54677..9.4132"),
            Tuple.Create("Hidden Quads 1", "65..87.24...649.5..4..25...57.438.61...5.1...31.9.2.85...89..1....213...13.75..98"),
            Tuple.Create("Hidden Quads 2", "9.15...46425.9..8186..1..2.5.2.......19...46.6.......2196.4.2532...6.817.....1694"),
            Tuple.Create("XWing 1", "1.....569492.561.8.561.924...964.8.1.64.1....218.356.4.4.5...169.5.614.2621.....5"),
            Tuple.Create("XWing 2", ".......9476.91..5..9...2.81.7..5..1....7.9....8..31.6724.1...7..1..9..459.....1..")
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
        if (puzzle_service.Grid.IsSolved())
            return;

        // Check if any cells have candidates (solver needs this)
        var any_candidates = puzzle_service.Grid.Cells.Any(c => c.Count() > 0);
        if (!any_candidates)
            puzzle_service.FillCandidates();

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
