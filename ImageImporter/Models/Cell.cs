using Emgu.CV.Structure;
using Emgu.CV;
using System.Diagnostics;
using System.Drawing;

namespace ImageImporter.Models;

[DebuggerDisplay("Cell {Id}")]
public class Cell
{
    public Image<Rgb, byte> Image { get; set; } = null!;
    public Point Center { get; set; } = new Point(0, 0);
    public Rectangle Bounds { get; set; } = new Rectangle();
    public int Id { get; set; } = 0;
}