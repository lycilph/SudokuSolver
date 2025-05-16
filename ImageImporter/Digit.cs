using Emgu.CV.Structure;
using Emgu.CV;
using System.Diagnostics;

namespace ImageImporter;

[DebuggerDisplay("Cell {Digit}, {Confidence}, {RecognitionFailure}")]
public class Digit(Cell cell)
{
    public Cell Cell { get; set; } = cell;
    public Image<Gray, byte> ImageProcessed { get; set; } = null!;
    public string Text { get; set; } = string.Empty;
    public float Confidence { get; set; } = 0f;
    public bool RecognitionFailure { get; set; } = false;
}