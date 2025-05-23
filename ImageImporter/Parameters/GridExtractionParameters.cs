namespace ImageImporter.Parameters;

public class GridExtractionParameters(int margin, int output_size)
{
    public int Margin = margin;
    public int OutputSize = output_size;

    public override string ToString() => $"Parameters: Margin={Margin}, Output size={OutputSize}";
}
