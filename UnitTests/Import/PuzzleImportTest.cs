using Core.Import;
using OpenCvSharp;

namespace UnitTests.Import;

public class PuzzleImportTest
{
    [Fact]
    public void ImportPuzzle()
    {
        var path = @"C:\Users\Morten Lang\source\repos\SudokuSolver\Data\Importer\";
        var image_filename = "IMG_20250330_101246.jpg";
        var puzzle_filename = Path.ChangeExtension(image_filename, "txt");

        var importer = new PuzzleImporter();
        var config = ImportConfiguration.Default();

        using (var t = new ResourcesTracker())
        {
            var image = t.T(new Mat(Path.Combine(path, image_filename)));
            var imported_puzzle = importer.Import(image, config);
            var actual_puzzle = File.ReadAllText(Path.Combine(path, puzzle_filename));

            Assert.Equal(imported_puzzle, actual_puzzle);
        }
    }
}
