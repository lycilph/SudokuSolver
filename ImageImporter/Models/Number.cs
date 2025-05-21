using Emgu.CV.Structure;
using Emgu.CV;
using System.Diagnostics;
using ImageImporter.Parameters;

namespace ImageImporter.Models;

[DebuggerDisplay("Number (Id={Cell.Id}) {Text}, {Confidence}, {RecognitionFailure}")]
public class Number(Cell cell)
{
    public Cell Cell { get; set; } = cell;
    public Image<Gray, byte> ImageProcessed { get; set; } = null!;
    public string Text { get; set; } = string.Empty;
    public float Confidence { get; set; } = 0f;
    public bool RecognitionFailure { get; set; } = false;
    public bool ContainsNumber { get; set; } = false;

    public NumberRecognitionParameters ParametersUsed { get; set; } = new();
}