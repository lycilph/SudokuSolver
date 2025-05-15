using Emgu.CV.Structure;
using Emgu.CV;
using System.Diagnostics;
using System.Drawing;

namespace ImageImporter;

[DebuggerDisplay("Cell {Id}")]
public class CellImage
{
    public Image<Rgb, byte> Image { get; set; } = null!;
    public Point Center { get; set; } = new Point(0, 0);
    public int Id { get; set; } = 0;
}
