namespace ImageImporter;

public struct CellsExtractionParameters(int threshold, int iterations)
{
    public int Threshold = threshold;
    public int Iterations = iterations;

    public override readonly string ToString() => $"Parameters: Threshold={Threshold}, Iterations={Iterations}";
}
