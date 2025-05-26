using OpenCvSharp;

namespace PaddleOCRUI.Core;

public class Cell
{
    public int Id {  get; set; }
    public Point2f Center { get; set; }
    public string Text { get; set; } = string.Empty;
}
