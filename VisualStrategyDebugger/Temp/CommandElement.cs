using Core.Model;
using System.Diagnostics;

namespace VisualStrategyDebugger.Temp;

[DebuggerDisplay("{Description}")]
public class CommandElement
{
    public string Description { get; set; } = string.Empty;
    public int Number { get; set; } = 0;
    public List<Cell> Cells { get; set; } = [];
}
