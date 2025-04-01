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
    private readonly VisualizationService visualization_service;
    private readonly UndoRedoService undo_service;
    private readonly GridViewModel gridVM;

    public ObservableCollection<IStrategy> Strategies { get; private set; } = [];
    public Dictionary<Type, IStrategyVisualizer> Visualizers { get; private set; } = [];

    //[ObservableProperty]
    //private ICommand? command = null;

    //[ObservableProperty]
    //private bool isShown = false;

    public SolverService(PuzzleService puzzle_service,
                         HighlightService highlight_service,
                         VisualizationService visualization_service,
                         UndoRedoService undo_service,
                         GridViewModel gridVM)
    {
        this.puzzle_service = puzzle_service;
        this.highlight_service = highlight_service;
        this.visualization_service = visualization_service;
        this.undo_service = undo_service;
        this.gridVM = gridVM;

        Strategies = Solver.KnownStrategies.ToObservableCollection();
        Visualizers = StrategyMapper.GetVisualizerMap(Strategies);
    }

    //public void Show() => IsShown = true;
    //public void Hide() => IsShown = false;

    //public bool NextHint()
    //{
    //    Command = Solver.Step(puzzle_service.Grid);
    //    return Command != null;
    //}

    //public void SetHint(ICommand cmd) => Command = cmd;

    //public void ExecuteCommand()
    //{
    //    if (Command != null)
    //        undo_service.Execute(Command);
    //}

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

    public void ShowVisualization()
    {
        // Needs to clear (potentially old) stuff
        //ClearVisualization();

        //if (Command != null && Command is BaseCommand base_command)
        //{
        //    var type = Command.GetType();
        //    var visualizer = Visualizers[type];

        //    // Show visualization here
        //    visualizer.Show(gridVM, base_command);
        //}
    }

    public void ClearVisualization()
    {
        highlight_service.Clear();
        visualization_service.Clear();
    }
}
