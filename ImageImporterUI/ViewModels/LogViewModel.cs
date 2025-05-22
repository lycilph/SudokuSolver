using System.IO;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ImageImporterUI.ViewModels;

public partial class LogViewModel(MainViewModel main) : ObservableObject
{
    public string Header { get; private set; } = "Log";

    [ObservableProperty]
    private string log = string.Empty;

    private string LoadActualPuzzles()
    {
        var puzzle_filename = MainViewModel.path + Path.ChangeExtension(main.SelectedImageFilename, ".txt");

        if (File.Exists(puzzle_filename))
            return File.ReadAllText(puzzle_filename);
        else 
            return string.Empty;
    }

    private string GetDifferences(string imported_puzzle, string actual_puzzle)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < imported_puzzle.Length; i++)
            sb.Append(imported_puzzle[i] == actual_puzzle[i] ? "." : "|");
        return sb.ToString();
    }

    public void Update()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Processing {main.SelectedImageFilename}");
        sb.Append(main.puzzle.DebugLog);

        var imported_puzzle = main.puzzle.Get();
        var actual_puzzle = LoadActualPuzzles();
        var differences = string.IsNullOrWhiteSpace(actual_puzzle) ? "no actual puzzle found" : GetDifferences(imported_puzzle, actual_puzzle);
        var differences_count = differences.Count(c => c == '|');

        // Statistics
        sb.AppendLine($"Processing image took: {main.TimeElapsed}");
        sb.AppendLine($"Imported puzzle: {imported_puzzle}");
        sb.AppendLine($"  Actual puzzle: {actual_puzzle}");
        sb.AppendLine($"    differences: {differences} (count {differences_count})");

        Log = sb.ToString();
    }
}
