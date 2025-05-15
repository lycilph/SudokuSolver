using System.Diagnostics;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageImportTest;

[DebuggerDisplay("Cell {Id}")]
public class Cell
{
    public Image<Rgb, byte> Image { get; set; } = null!;
    public Point Center { get; set; } = new Point(0, 0);
    public int Id { get; set; } = 0;
}
