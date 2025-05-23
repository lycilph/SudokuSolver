namespace ImageImporter.Parameters;

public class CellsExtractionParameters(int threshold, int iterations)
{
    public int Threshold = threshold;
    public int Iterations = iterations;

    public override string ToString() => $"Parameters: Threshold={Threshold}, Iterations={Iterations}";
}
