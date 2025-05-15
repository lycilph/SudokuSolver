using Emgu.CV.Structure;
using Emgu.CV;
using System.Diagnostics;

namespace ImageImporter;

[DebuggerDisplay("Cell {Digit}, {Confidence}, {RecognitionFailure}")]
public class CellDigit(CellImage cell_image)
{
    public CellImage CellImage { get; set; } = cell_image;
    public Image<Gray, byte> CellImageProcessed { get; set; } = null!;
    public string Digit { get; set; } = string.Empty;
    public float Confidence { get; set; } = 0f;
    public bool RecognitionFailure { get; set; } = false;
}
