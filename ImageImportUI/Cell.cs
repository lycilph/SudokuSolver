using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageImportUI;

public class Cell
{
    public Image<Rgb, byte> Image { get; set; } = null!;
    public Image<Gray, byte> Processed { get; set; } = null!;
    public Point Center { get; set; } = new Point(0, 0);
    public string Digit { get; set; } = string.Empty;
    public bool RecognitionFailed { get; set; } = false;
}
