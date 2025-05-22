using ImageImporter;
using ImageImporter.Parameters;

namespace Sandbox;

internal class Program
{
    static void Main()
    {
        //var path = "C:\\Users\\Morten Lang\\source\\repos\\SudokuSolver\\Data\\Importer\\";
        //var files = Directory
        //    .EnumerateFiles(path)
        //    .Where(f => Path.GetExtension(f) == ".jpg")
        //    .ToList();
        //var parameters = new ImportParameters();

        //var importer = new Importer();
        //var puzzle = importer.Import(files.First(), parameters);
        //Console.WriteLine($"Puzzle: {puzzle.Get()}");

        //Console.WriteLine("Debug log:");
        //Console.WriteLine(puzzle.DebugLog);

        //Console.WriteLine("Result log:");
        //Console.WriteLine(puzzle.ResultLog);

        var filename = $"WebcamCapture_{DateTime.Now.ToString()}.jpg"
            .Replace("-", "")
            .Replace(":", "")
            .Replace(" ", "_");
        Console.WriteLine(filename);


        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
