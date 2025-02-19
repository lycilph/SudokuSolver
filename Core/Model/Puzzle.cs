namespace Core.Model;

public class Puzzle
{
    public Grid Grid { get; set; } = new Grid();
    public List<ISolveAction> Actions { get; } = [];
    public Statistics Stats { get; } = new Statistics();

    public bool IsSolved() => Grid.IsSolved();

    public Puzzle(string puzzle)
    {
        Grid = new Grid(puzzle);
    }
}
