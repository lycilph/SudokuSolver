using System.Windows.Media;
using Core.Models;

namespace SudokuUI.Visualizers.Graphics;

public class Link(Cell start, Cell end, Link.LineType line_type = Link.LineType.Solid)
{
    public enum LineType { Solid, Dotted };

    public Cell Start { get; set; } = start;
    public Cell End { get; set; } = end;
    public LineType Style { get; set; } = line_type;
    public Brush Color { get; set; } = Brushes.Black;
}
