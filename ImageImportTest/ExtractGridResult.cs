using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageImportTest;

public class ExtractGridResult
{
    public Image<Rgb, byte> OutputImage { get; set; } = null!;
    public string Log { get; set; } = string.Empty;
}
