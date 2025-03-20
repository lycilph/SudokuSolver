using Core.Extensions;
using Core.Models;

namespace SudokuUI.Services;

public class PuzzleService
{
    public Grid Grid { get; private set; } = new Grid();

    public PuzzleService()
    {
        // DEBUG
        Grid.Load(".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4"); // These numbers should show as clues
        Grid[0].Value = 9; // To check non-clue visualization
        Grid.Cells.Where(c => c.Value == 0).ForEach(c => c.Candidates.UnionWith([.. Grid.PossibleValues])); // To check candidates visualization
        Grid[2].Candidates.Remove(5);
    }
}
