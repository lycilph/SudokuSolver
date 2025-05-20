namespace ImageImporter.Parameters;

public struct GridExtractionParameters(int margin, int output_size)
{
    public int Margin = margin;
    public int OutputSize = output_size;

    public override readonly string ToString() => $"Parameters: Margin={Margin}";
}
