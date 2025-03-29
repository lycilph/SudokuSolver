using System.Windows.Media;
using Core.Models;

namespace SudokuUI.Visualizers.Misc;

public class LinkVisualizer(Cell start, Cell end, Brush color, LinkVisualizer.LineType line_type = LinkVisualizer.LineType.Solid)
{
    public enum LineType { Solid, Dotted };

    public Cell Start { get; set; } = start;
    public Cell End { get; set; } = end;
    public Brush Color { get; set; } = color;
    public LineType Style { get; set; } = line_type;
}
