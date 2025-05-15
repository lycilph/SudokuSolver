using Emgu.CV.Structure;
using Emgu.CV;
using System.Diagnostics;

namespace ImageImportTest;

internal class Program
{
    static void Main()
    {
        string path = "C:\\Users\\Morten Lang\\source\\repos\\SudokuSolver\\ImageImportTest\\Data\\";
        List<string> image_files =
            Directory
            .EnumerateFiles(path)
            .ToList();
        //List<string> image_files = ["C:\\Users\\Morten Lang\\source\\repos\\SudokuSolver\\ImageImportTest\\Data\\IMG_20250330_101246.jpg"];
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
            var recognition_failures = extract_digit_results.Count(r => r.RecognitionFailure);
            var digits_found = extract_digit_results.Count(r => !string.IsNullOrWhiteSpace(r.Digit));
            //extract_digit_results.ForEach(r => Console.WriteLine($" * {r.Log}"));
            Console.WriteLine($" * Found {digits_found} and failed recognizing {recognition_failures} cells");

            // Get a list of known digits
            var known_digits = extract_digit_results
                .Select(r => new { Id = r.Input.Id, Cell = r.Input, Digit = r.Digit, Confidence = r.Confidence })
                .ToList();

            // Cleaning up digits
            var digit_parameters = new[]
            {
                new { Threshold = 5, KernelSize = 2, Iterations = 1, Operation = 1 },
                new { Threshold = 2, KernelSize = 3, Iterations = 1, Operation = 1 },
                new { Threshold = 3, KernelSize = 1, Iterations = 1, Operation = 1 },
                new { Threshold = 1, KernelSize = 5, Iterations = 1, Operation = 1 },
                new { Threshold = 5, KernelSize = 5, Iterations = 1, Operation = 2 }
            }.ToList();
            var failed_cell_results = extract_digit_results.Where(c => c.RecognitionFailure).ToList();
            foreach ( var cell_result in failed_cell_results)
            {
                var results = digit_parameters
                    .Select(p => importer.CleanupCell(cell_result.Input, p.Threshold, p.KernelSize, p.Iterations, p.Operation))
                    .ToList();

                // Check if any of the attempts above resulted in a match
                var best_match = results
                    .Where(r => !r.RecognitionFailure)
                    .OrderByDescending(r => r.Confidence)
                    .FirstOrDefault();

                if (best_match != null )
                    known_digits[cell_result.Input.Id] = new { Id = best_match.Input.Id, Cell = best_match.Input, Digit = best_match.Digit, Confidence = best_match.Confidence };
            }
            var final_digits_found = known_digits.Count(d => !string.IsNullOrWhiteSpace(d.Digit));
            Console.WriteLine($" * After clean up {final_digits_found} was recognized");

            // Write out all recognized digits
            foreach (var digit in known_digits.Where(d => !string.IsNullOrWhiteSpace(d.Digit)))
                Console.WriteLine($" * Found digit {digit.Digit} in cell {digit.Id} with confidence {digit.Confidence}");

            // Statistics
            stop_watch.Stop();
            Console.WriteLine($" * Processing image took: {stop_watch.ElapsedMilliseconds}ms");
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
