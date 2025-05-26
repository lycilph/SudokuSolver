namespace PaddleOCRUI.Core;

public readonly struct ImportConfiguration(int threshold, int block_size, int grid_output_size, bool debug)
{
    public int Threshold { get; } = threshold;
    public int BlockSize { get; } = block_size;
    public int GridOutputSize { get; } = grid_output_size;
    public bool Debug { get; } = debug;

    public static ImportConfiguration DefaultConfig() => new ImportConfiguration(5, 55, 3000, false);
    public static ImportConfiguration DebugConfig() => new ImportConfiguration(5, 55, 3000, true);
}
