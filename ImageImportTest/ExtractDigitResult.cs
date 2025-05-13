using Emgu.CV.Structure;
using Emgu.CV;

namespace ImageImportTest;

public class ExtractDigitResult(Cell input)
{
    public Cell Input { get; set; } = input;
    public Image<Gray, byte> Processed { get; set; } = null!;
    public string Digit { get; set; } = string.Empty;
    public bool RecognitionSuccess { get; set; } = false;
    public float Confidence { get; set; } = 0f;
    public string Log { get; set; } = string.Empty;
}
