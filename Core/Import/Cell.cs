using System.Diagnostics;
using OpenCvSharp;

namespace Core.Import;

[DebuggerDisplay("Cell {Id} ({Center.X},{Center.Y}) [{Text}]")]
public class Cell
{
    public int Id {  get; set; }
    public Point2f Center { get; set; }
    public string Text { get; set; } = string.Empty;
}
