using System.ComponentModel;
using Core.Extensions;
using Core.Models;
using NLog;
using SudokuUI.Infrastructure;

namespace SudokuUI.Services;

public class PuzzleService
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public Grid Grid { get; private set; } = new Grid();
    
    public event EventHandler<GridValuesChangedEventArgs> ValuesChanged = null!;

    public PuzzleService()
    {
        // DEBUG
        Grid.Load(".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4"); // These numbers should show as clues
        Grid[0].Value = 9; // To check non-clue visualization
        Grid.Cells.Where(c => c.Value == 0).ForEach(c => c.Candidates.UnionWith([.. Grid.PossibleValues])); // To check candidates visualization
        Grid[2].Candidates.ExceptWith([5,6,7]);

        foreach (var cell in Grid.Cells)
            cell.PropertyChanged += CellChanged;
    }

    private void CellChanged(object? sender, PropertyChangedEventArgs e)
    {
        logger.Trace("Grid values changed");

        Grid.Cells.GroupBy(c => c.Value).ForEach(g => g)

        ValuesChanged?.Invoke(this, );
    }
}
