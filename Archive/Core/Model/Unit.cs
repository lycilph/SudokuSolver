using System.Diagnostics;

namespace Core.Model;

public enum UnitType { Row, Column };

[DebuggerDisplay("{FullName}")]
public class Unit
{
    public string Name { get; set; } = string.Empty;
    public int Index { get; set; }
    public Cell[] Cells { get; set; } = [];

    public string FullName => $"{Name} {Index}";

    public IEnumerable<Cell> FilledCells() => Cells.Where(c => c.IsFilled);
    public IEnumerable<Cell> EmptyCells() => Cells.Where(c => c.IsEmpty);
}
