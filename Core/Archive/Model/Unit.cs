using System.Diagnostics;

namespace Core.Archive.Model;

[DebuggerDisplay("{FullName}")]
public class Unit
{
    public string Name { get; set; } = string.Empty;

    public int Index { get; set; }

    public Cell[] Cells { get; set; } = [];

    public string FullName => $"{Name} {Index}";
}
