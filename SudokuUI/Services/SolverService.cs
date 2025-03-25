using System.Collections.ObjectModel;
using Core.Commands;
using Core.Strategies;
using SudokuUI.Visualizers;

namespace SudokuUI.Services;

public class SolverService
{
    public ObservableCollection<IStrategy> Strategies { get; private set; } =
        [
            new BasicEliminationStrategy(),
            new NakedSinglesStrategy(),
            new HiddenSinglesStrategy()
        ];

    public Dictionary<Type, IStrategyVisualizer> Visualizers { get; private set; } = [];

    public SolverService()
    {
        Visualizers.Add(typeof(BasicEliminationCommand), new BasicEliminationVisualizer());
        Visualizers.Add(typeof(NakedSinglesCommand), new NakedSinglesVisualizer());
        Visualizers.Add(typeof(HiddenSinglesCommand), null!);
    }
}
