using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageImporter.Models;

public class Puzzle(string filename)
{
    private readonly StringBuilder debug_log = new();
    private readonly StringBuilder result_log = new();

    public string Filename { get; private set; } = filename;

    public Image<Rgb, byte> InputImage { get; set; } = null!;
    public Grid Grid { get; set; } = new Grid();
    public List<Cell> Cells { get; set; } = [];
    public List<Number> Numbers { get; set; } = [];

    public string DebugLog => debug_log.ToString();
    public string ResultLog => result_log.ToString();

    public void AppendDebugLog(string msg) => debug_log.AppendLine(msg);
    public void AppendResultLog(string msg) => result_log.AppendLine(msg);

    public string Get()
    {
        return string.Empty;
    }
}
