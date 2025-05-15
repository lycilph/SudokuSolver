using System.Diagnostics;
using System.Text;

namespace ImageImporter;

internal class Program
{
    static void Main(string[] args)
    {
        string path = "C:\\Users\\Morten Lang\\source\\repos\\SudokuSolver\\ImageImportTest\\Data\\";
        //List<string> image_files =
        //    Directory
        //    .EnumerateFiles(path)
        //    .ToList();
        //List<string> image_files = [$"{path}image1005.jpg"];
        List<string> image_files = [$"{path}IMG_20250330_101246.jpg"];
        Console.WriteLine($"Found {image_files.Count} image files to process");

        var importer = new ImageImporter();
        foreach (var file in image_files)
        {
            var stop_watch = Stopwatch.StartNew();
            Console.WriteLine($"Processing {file}");

            importer.Import(file);
            Console.WriteLine(importer.Log);

            var puzzle_filename = Path.ChangeExtension(file, ".txt");
            var puzzle = File.ReadAllText(puzzle_filename);
            var imported_puzzle = importer.GetPuzzle();

            var sb = new StringBuilder();
            for (int i = 0; i < puzzle.Length; i++)
                sb.Append(puzzle[i] == imported_puzzle[i] ? "." : "|");
            var differences = sb.ToString();
            var differences_count = differences.Count(c => c == '|');

            // Statistics
            stop_watch.Stop();
            Console.WriteLine($"Processing image took: {stop_watch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Imported puzzle: {imported_puzzle}");
            Console.WriteLine($"  Actual puzzle: {puzzle}");
            Console.WriteLine($"    differences: {differences}");
            Console.WriteLine($"Differences count: {differences_count}");
            // Check for differences


        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
