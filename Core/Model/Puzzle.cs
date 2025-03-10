using Core.Model.Actions;

namespace Core.Model;

public class Puzzle
{
    public Grid Grid { get; set; } = new Grid();
    public List<IPuzzleAction> Actions { get; } = [];
    public Statistics Stats { get; } = new Statistics();

    public bool IsSolved() => Grid.IsSolved();

    public Puzzle() { }

    public Puzzle(string puzzle)
    {
        Grid = new Grid(puzzle);
    }

    public void Set(string puzzle) => Grid.Set(puzzle);
}
