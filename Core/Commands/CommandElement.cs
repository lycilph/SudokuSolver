using Core.Models;
using System.Diagnostics;

namespace Core.Commands;

[DebuggerDisplay("{Description}")]
public class CommandElement
{
    public string Description { get; set; } = string.Empty;
    public List<int> Numbers { get; set; } = [];
    public List<Cell> Cells { get; set; } = [];

    public List<int> NumbersToVisualize { get; set; } = [];
    public List<Cell> CellsToVisualize { get; set; } = [];

    public int Number => Numbers.Single();
    public Cell Cell => Cells.Single();
}
