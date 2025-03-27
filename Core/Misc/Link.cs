using Core.Models;

namespace Core.Misc;

public class Link(Cell start, Cell end)
{
    public Cell Start { get; set; } = start;
    public Cell End { get; set; } = end;

    public override string ToString() => $"({Start.Index},{End.Index})";
}
