using Core.Models;

namespace Core.Misc;

public class Link(int value, Cell start, Cell end)
{
    public int Value { get; set; } = value;
    public Cell Start { get; set; } = start;
    public Cell End { get; set; } = end;

    public override string ToString() => $"({Start.Index},{End.Index})";
}
