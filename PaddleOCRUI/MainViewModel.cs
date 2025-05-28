using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenCvSharp;
using PaddleOCRUI.Core;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace PaddleOCRUI;

// https://www.thomasclaudiushuber.com/2025/02/21/wpf-in-net-9-0-windows-11-theming/

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

            Debug.WriteLine($"Imported puzzle: {importer.MapNumbersToCells(regions, grid.Size())}");
        }

        OnPropertyChanged(nameof(Image));
        OnPropertyChanged(nameof(ProcessedImage));
    }

    partial void OnImageChanging(Mat value)
    {
        if (image != null)
            image.Dispose();
    }

    partial void OnProcessedImageChanging(Mat value)
    {
        if (ProcessedImage != null)
            ProcessedImage.Dispose();
    }

    [RelayCommand]
    private void Capture()
    {
        var win = new CaptureImageWindow { Owner = App.Current.MainWindow };
        win.ShowDialog();
    }
}
