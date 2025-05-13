using Emgu.CV.Structure;
using Emgu.CV;
using System.Diagnostics;

namespace ImageImportTest;

internal class Program
{
    static void Main()
    {
        string path = "C:\\Users\\Morten Lang\\source\\repos\\SudokuSolver\\ImageImportTest\\Data\\";
        //List<string> image_files =
        //    Directory
        //    .EnumerateFiles(path)
        //    .ToList();
        List<string> image_files = ["C:\\Users\\Morten Lang\\source\\repos\\SudokuSolver\\ImageImportTest\\Data\\IMG_20250330_101246.jpg"];
        Console.WriteLine($"Found {image_files.Count} image files to process");

        var importer = new ImageImporter();
        foreach (var file in image_files)
        {
            var stop_watch = Stopwatch.StartNew();

            Console.WriteLine($"Processing {file}");

            // Load image
            var input_image = new Image<Rgb, byte>(file);

            // Extracting grid
            var extract_grid_result = importer.ExtractGrid(input_image, 0);
            //Console.WriteLine(extract_grid_result.Log);

            // Extracting cells
            var cells_parameters = new[]
            {
                new { Threshold = 5, Iterations = 1 },
                new { Threshold = 5, Iterations = 3 },
                new { Threshold = 2, Iterations = 3 }
            }.ToList();
            ExtractCellsResult extract_cells_result = new();
            foreach (var parameters in cells_parameters)
            {
                try
                {
                    extract_cells_result = importer.ExtractCells(extract_grid_result.OutputImage, parameters.Threshold, parameters.Iterations);
                    if (extract_cells_result.Count == 81)
                    {
                        Console.WriteLine($" * Found {extract_cells_result.Count} cells in the image (parameters: threshold={parameters.Threshold}, iterations={parameters.Iterations})");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" * Error {ex.Message}");
                }
            }
            if (extract_cells_result.Count != 81)
            {
                Console.WriteLine($" * !!!Couldn't find 81 cells in the image (skipping this file)!!!");
                continue;
            }
            //Console.WriteLine(extract_cells_result.Log);

            
            // Extracting digits
            var extract_digit_results = importer.ExtractDigits(extract_cells_result.Cells);
            /*var recognition_failures = cells.Count(c => c.RecognitionFailed);
            var digits_found = cells.Count(c => !string.IsNullOrWhiteSpace(c.Digit));
            Console.WriteLine($" * Found {digits_found} and failed recognizing {recognition_failures} cells");

            // Cleaning up digits
            var digit_parameters = new[]
            {
                new { Threshold = 5, KernelSize = 2, Iterations = 1, Operation = 1 },
                new { Threshold = 2, KernelSize = 3, Iterations = 1, Operation = 1 },
                new { Threshold = 3, KernelSize = 1, Iterations = 1, Operation = 1 },
                new { Threshold = 1, KernelSize = 5, Iterations = 1, Operation = 1 },
                new { Threshold = 5, KernelSize = 5, Iterations = 1, Operation = 2 }
            }.ToList();
            var failed_cells = cells.Where(c => c.RecognitionFailed).ToList();
            foreach (var cell in failed_cells)
            {
                foreach (var parameter in digit_parameters)
                {
                    importer.CleanupCell(cell, parameter.Threshold, parameter.KernelSize, parameter.Iterations, parameter.Operation);
                    if (!cell.RecognitionFailed)
                    {
                        Console.WriteLine($" * Recognized digit {cell.Digit} (confidence={cell.Confidence}, parameters: threshold={parameter.Threshold}, kernel size={parameter.KernelSize}, iterations={parameter.Iterations}, operations={parameter.Operation})");
                        break;
                    }
                }
                if (cell.RecognitionFailed)
                    Console.WriteLine($" * !!!Couldn't recognize any digits in the cell (skipping cell)!!!");
            }
            recognition_failures = cells.Count(c => c.RecognitionFailed);
            if (recognition_failures > 0)
                Console.WriteLine($" * !!!A total of {recognition_failures} cells were not recognized!!!");
            */
            // Statistics
            stop_watch.Stop();
            Console.WriteLine($" * Processing image took: {stop_watch.ElapsedMilliseconds}ms");
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
