using System.Diagnostics;

namespace Core.Models;

public enum UnitType { Row, Column };

[DebuggerDisplay("{FullName}")]
public class Unit
{
    public string Name { get; set; } = string.Empty;
    public int Index { get; set; }
    public Cell[] Cells { get; set; } = [];

    public string FullName => $"{Name} {Index}";
}
