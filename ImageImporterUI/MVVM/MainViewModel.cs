using System.Diagnostics;
using System.IO;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ImageImporter;

namespace ImageImporterUI.MVVM;

public partial class MainViewModel : ObservableRecipient, IRecipient<string>
{
    private  static string path = "C:\\Users\\Morten Lang\\source\\repos\\SudokuSolver\\Data\\Importer\\";

    private readonly Importer importer = new();

    [ObservableProperty]
    private List<string> imageFilenames;

    [ObservableProperty]
    private string selectedImageFilename;

    [ObservableProperty]
    private string log = string.Empty;

    [ObservableProperty]
    private bool isBusy = false;

    public MainViewModel()
    {
        ImageFilenames = [.. Directory
            .EnumerateFiles(path)
            .Where(f => Path.GetExtension(f) == ".jpg")
            .Select(s => Path.GetFileName(s))];
        //SelectedImageFilename = ImageFilenames.First();

        IsActive = true; // Needed to recieve messages
    }

    public void Receive(string message)
    {
        if (string.IsNullOrWhiteSpace(SelectedImageFilename))
            SelectedImageFilename = ImageFilenames.First();
    }

    partial void OnSelectedImageFilenameChanged(string value)
    {
        _ = Import();
    }

    [RelayCommand]
    private async Task Import()
    {
        IsBusy = true;

        var stop_watch = Stopwatch.StartNew();
        var sb = new StringBuilder();
        sb.AppendLine($"Processing {SelectedImageFilename}");

        await Task.Run(() => 
        {
            importer.Import(path + SelectedImageFilename);
        });
        sb.Append(importer.Log);

        var puzzle_filename = Path.ChangeExtension(SelectedImageFilename, ".txt");
        var puzzle = File.ReadAllText(path+puzzle_filename);
        var imported_puzzle = importer.GetPuzzle();

        var puzzle_sb = new StringBuilder();
        for (int i = 0; i < puzzle.Length; i++)
            puzzle_sb.Append(puzzle[i] == imported_puzzle[i] ? "." : "|");
        var differences = puzzle_sb.ToString();
        var differences_count = differences.Count(c => c == '|');

        // Statistics
        stop_watch.Stop();
        sb.AppendLine($"Processing image took: {stop_watch.ElapsedMilliseconds}ms");
        sb.AppendLine($"Imported puzzle: {imported_puzzle}");
        sb.AppendLine($"  Actual puzzle: {puzzle}");
        sb.AppendLine($"    differences: {differences} (count {differences_count})");

        Log = sb.ToString();

        IsBusy = false;
    }
}
