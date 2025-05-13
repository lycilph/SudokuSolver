using Emgu.CV.Structure;
using Emgu.CV;

namespace ImageImportTest;

public class ExtractCellsResult
{
    public Image<Rgb, byte> OutputImage { get; set; } = null!;
    public List<Cell> Cells { get; set; } = [];
    public string Log { get; set; } = string.Empty;

    public int Count => Cells.Count;
}
