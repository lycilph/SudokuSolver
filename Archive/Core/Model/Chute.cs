using System.Diagnostics;

namespace Core.Model;

[DebuggerDisplay("{FullName}")]
public class Chute
{
    public string Name { get; set; } = string.Empty;
    public int Index { get; set; }
    public Unit[] Boxes { get; set; } = [];

    public string FullName => $"{Name} {Index}";

    public Cell[] Cells => [.. Boxes.SelectMany(b => b.Cells)];
}
