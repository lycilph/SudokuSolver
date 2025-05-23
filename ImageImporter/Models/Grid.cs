using Emgu.CV;
using Emgu.CV.Structure;
using ImageImporter.Parameters;

namespace ImageImporter.Models;

public class Grid
{
    public Image<Rgb, byte> Image { get; set; } = null!;
    public Image<Rgb, byte> DebugImage { get; set; } = null!;
    public GridExtractionParameters ParametersUsed { get; set; } = null!;
}
