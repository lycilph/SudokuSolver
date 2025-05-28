using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenCvSharp;
using PaddleOCRUI.Core;

namespace PaddleOCRUI;

public partial class MainViewModel : ObservableObject
{
    public static string path = "C:\\Users\\Morten Lang\\source\\repos\\SudokuSolver\\Data\\Importer\\";
    //private static string path = @"C:\Users\morte\Projects\SudokuSolver\Data\Importer\";

    private readonly PuzzleImporter importer = new();
    private readonly ImportConfiguration config = ImportConfiguration.Default();

    [ObservableProperty]
    private ObservableCollection<string> imageFilenames;

    [ObservableProperty]
    private string selectedImageFilename = string.Empty;

    [ObservableProperty]
    private Mat image = null!;

    [ObservableProperty]
    private Mat processedImage = null!;

    [ObservableProperty]
    private string log = string.Empty;

    public MainViewModel()
    {
        ImageFilenames = [.. Directory
            .EnumerateFiles(path)
            .Where(f => Path.GetExtension(f) == ".jpg")
            .Select(s => Path.GetFileName(s))];
        SelectedImageFilename = ImageFilenames.First();
    }

    partial void OnSelectedImageFilenameChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
            return;

        Image = new Mat(Path.Combine(path, SelectedImageFilename));

        using (var grid = importer.ExtractGrid(Image, config))
        {
            var regions = importer.RecognizeNumbers(grid);
            ProcessedImage = importer.VisualizeDetection(grid, regions);

            var imported_puzzle = importer.MapNumbersToCells(regions, grid.Size());
            UpdateLog(imported_puzzle);
        }
    }
    
    private string GetDifferences(string imported_puzzle, string actual_puzzle)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < imported_puzzle.Length; i++)
            sb.Append(imported_puzzle[i] == actual_puzzle[i] ? "." : "|");
        return sb.ToString();
    }

    private void UpdateLog(string imported_puzzle)
    {
        var filename = Path.Combine(path, Path.ChangeExtension(SelectedImageFilename, "txt"));
        if (File.Exists(filename))
        {
            var actual_puzzle = File.ReadAllText(filename);
            var differences = string.IsNullOrWhiteSpace(actual_puzzle) ? "no actual puzzle found" : GetDifferences(imported_puzzle, actual_puzzle);
            var differences_count = differences.Count(c => c == '|');

            var sb = new StringBuilder();
            sb.AppendLine($"Processing {SelectedImageFilename}");

            // Statistics
            sb.AppendLine($"Imported puzzle: {imported_puzzle}");
            sb.AppendLine($"  Actual puzzle: {actual_puzzle}");
            sb.AppendLine($"    differences: {differences} (count {differences_count})");

            Log = sb.ToString();
        }
    }

    partial void OnImageChanging(Mat value)
    {
        Image?.Dispose();
    }

    partial void OnProcessedImageChanging(Mat value)
    {
        ProcessedImage?.Dispose();
    }

    [RelayCommand]
    private void Capture()
    {
        var win = new CaptureImageWindow { Owner = App.Current.MainWindow };
        win.ShowDialog();
    }
}
