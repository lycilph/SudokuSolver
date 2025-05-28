namespace Core.Import;

public readonly struct ImportConfiguration(int threshold, int block_size, int grid_output_size)
{
    public int Threshold { get; } = threshold;
    public int BlockSize { get; } = block_size;
    public int GridOutputSize { get; } = grid_output_size;

    public static ImportConfiguration Default() => new(5, 55, 3000);
}
