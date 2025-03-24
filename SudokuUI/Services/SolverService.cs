using System.Collections.ObjectModel;
using Core.Strategies;

namespace SudokuUI.Services;

public class SolverService
{
    public ObservableCollection<IStrategy> Strategies { get; private set; } =
        [
            new BasicEliminationStrategy(),
            new NakedSinglesStrategy()
        ];
}
