using OpenCvSharp;

namespace SudokuUI.Dialogs.ImageImport;

public class ImportStepOutput
{
    public enum StepResult { SelectImage, CaptureImage, Next, Done, Cancelled }
    
    public string Filename { get; set; } = string.Empty;
    public string PuzzleSource { get; set; } = string.Empty;    
    public Mat Image { get; set; } = null!;
    public StepResult Result { get; set; }

    public static ImportStepOutput SelectImage(string filename) => new() { Filename = filename, Result = StepResult.SelectImage };
    public static ImportStepOutput CaptureImage() => new() { Result = StepResult.CaptureImage };
    public static ImportStepOutput Next(Mat image) => new() { Image = image, Result = StepResult.Next };
    public static ImportStepOutput Done(string puzzle_source) => new() { PuzzleSource = puzzle_source, Result = StepResult.Done };
    public static ImportStepOutput Cancel() => new() { Result = StepResult.Cancelled };
}
