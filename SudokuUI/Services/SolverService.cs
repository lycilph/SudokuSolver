using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Core.Commands;
using Core.Engine;
using Core.Extensions;
using Core.Strategies;
using SudokuUI.Infrastructure;
using SudokuUI.Messages;
using SudokuUI.ViewModels;
using SudokuUI.Visualizers;

namespace SudokuUI.Services;

public partial class SolverService : ObservableObject
{
    private readonly PuzzleService puzzle_service;
    private readonly HighlightService highlight_service;
    private readonly UndoRedoService undo_service;
    private readonly GridViewModel gridVM;

    public ObservableCollection<IStrategy> Strategies { get; private set; } = [];
    public Dictionary<Type, IStrategyVisualizer> Visualizers { get; private set; } = [];

    public event EventHandler<BaseCommand> VisualizeCommandRequest = null!;

    public SolverService(PuzzleService puzzle_service,
                         HighlightService highlight_service,
                         UndoRedoService undo_service,
                         GridViewModel gridVM)
    {
        this.puzzle_service = puzzle_service;
        this.highlight_service = highlight_service;
        this.undo_service = undo_service;
        this.gridVM = gridVM;

        Strategies = Solver.KnownStrategies.ToObservableCollection();
        Visualizers = StrategyMapper.GetVisualizerMap(Strategies);
    }

    public void VisualizeCommand(BaseCommand command) => VisualizeCommandRequest?.Invoke(this, command);

    public bool HasNextHint()
    {
        return Solver.Step(puzzle_service.Grid) != null;
    }

    public BaseCommand? NextCommand()
    {
        if (Solver.Step(puzzle_service.Grid) is BaseCommand cmd)
            return cmd;
        else
            return null;
    }

    public void Execute(BaseCommand cmd) => undo_service.Execute(cmd);

    public void SolveNakedSingles()
    {
        while (true)
        {
            var command = NakedSinglesStrategy.Instance.Plan(puzzle_service.Grid);
            if (command != null)
            {
                undo_service.Execute(command);

                command = BasicEliminationStrategy.Instance.Plan(puzzle_service.Grid);
                if (command != null)
                    undo_service.Execute(command);
            }
            else
            {
                if (!puzzle_service.Grid.IsSolved())
                    WeakReferenceMessenger.Default.Send(new ShowNotificationMessage("No more naked singles found"));
                return;
            }
        }
    }

    public void ShowVisualization(BaseCommand cmd, int index = -1)
    {
        // Needs to clear (potentially old) stuff
        ClearVisualization();

        var type = cmd.GetType();
        var visualizer = Visualizers[type];

        if (index >= 0)
            visualizer.Show(gridVM, cmd.Elements[index]);
        else
            // Show visualization here
            visualizer.Show(gridVM, cmd);
    }

    public void ClearVisualization()
    {
        highlight_service.Clear();
    }
}
