namespace ImageImporter.Parameters;

public class NumberRecognitionParameters(int threshold, int kernel_size, int iterations, int operation)
{
    public int Threshold = threshold;
    public int KernelSize = kernel_size;
    public int Iterations = iterations;
    public int Operation = operation;

    public override string ToString() => $"Parameters: Threshold={Threshold}, Kernel Size={KernelSize}, Iterations={Iterations}, Operation={Operation}";
}