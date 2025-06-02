using Core.Import;
using NLog;
using OpenCvSharp;
using SudokuUI.Dialogs.ImageImport;

namespace SudokuUI.Services;

public class ImageImportService
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private PuzzleImporter? importer;
    private ImportConfiguration config;

    public Mat ProcessedImage { get; private set; } = null!;
    public string PuzzleSource { get; private set; } = string.Empty;

    public Task Initialize()
    {
        logger.Info("Image Import Service Initializing");

        if (importer is not null)
        {
            logger.Warn("Image Importer is already initialized. Skipping initialization");
            return Task.CompletedTask;
        }

        return Task.Run(() =>
        {
            // Initialize the PuzzleImporter instance
            importer = new PuzzleImporter();
            config = ImportConfiguration.Default();
            logger.Info("Puzzle Importer initialized successfully.");
        });
    }

    public Task ProcessImage(Mat input, bool dispose_image = true)
    {
        if (importer is null)
        {
            logger.Error("Image Importer is not initialized. Please call Initialize() first");
            return Task.CompletedTask;
        }

        return Task.Run(() =>
        {
            // Process the input image using the importer
            using (var grid = importer.ExtractGrid(input, config))
            {
                var regions = importer.RecognizeNumbers(grid);

                ProcessedImage?.Dispose(); // Dispose old image (if not null)
                ProcessedImage = importer.VisualizeDetection(grid, regions);

                if (regions.Length > 10)
                {
                    PuzzleSource = importer.MapNumbersToCells(regions, grid.Size());
                    logger.Info($"Puzzle: {PuzzleSource}");
                }
                else
                    logger.Info("It doesn't seem like the image is a sudoku puzzle...");
            }

            if (dispose_image)
                input.Dispose();
            logger.Info("Image processed successfully");
        });
    }

    public void Cleanup()
    {
        ProcessedImage?.Dispose();
        PuzzleSource = string.Empty;
    }

    public IEnumerable<ImageImportStep> Show()
    {
        if (importer is null)
        {
            logger.Error("Image Importer is not initialized. Please call Initialize() first.");
            yield break; // Exit if the importer is not initialized
        }

        // Logic to show the image import dialogs
        logger.Info("Image import Started");

        // Show the dialog to select an image file OR capture from camera
        var vm = new ImageImportDialogViewModel();
        var view = new ImageImportDialogView { DataContext = vm };
        yield return new ImageImportStep(vm, view, "Image Import");

        var output = vm.DialogResult.Result;
        if (output.Result == ImportStepOutput.StepResult.Cancelled)
        {
            logger.Info("Image import cancelled by user.");
            yield break;
        }

        // Dependending on the selected option, either process the image file or capture from camera
        logger.Info($"Import method selected: {output}");
        if (output.Result == ImportStepOutput.StepResult.CaptureImage)
        {
            var capture_vm = new CaptureImageImportDialogViewModel();
            var capture_view = new CaptureImageImportDialogView { DataContext = capture_vm };
            yield return new ImageImportStep(capture_vm, capture_view, "Capture Image");

            output = capture_vm.DialogResult.Result;
        }

        if (output.Result == ImportStepOutput.StepResult.Cancelled)
        {
            logger.Info("Image import cancelled by user.");
            yield break;
        }

        // Verify the processed image and the extracted Sudoku grid
        var verify_vm = new VerifyImageImportDialogViewModel(ProcessedImage, PuzzleSource);
        var verify_view = new VerifyImageImportDialogView { DataContext = verify_vm };
        yield return new ImageImportStep(verify_vm, verify_view, "Verify Image");
    }
}
