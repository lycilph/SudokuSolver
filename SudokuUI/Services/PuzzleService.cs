using Core.Model;
using Core.Strategy;

namespace SudokuUI.Services;

public class PuzzleService
{
    private string input = "4......38.32.941...953..24.37.6.9..4.29..16.36.47.3.9.957..83....39..4..24..3.7.9";

    public Grid Grid { get; private set; }

    public PuzzleService()
    {
        Grid = new Grid(input);
        BasicEliminationStrategy.Temp(Grid);
    }
}
