using Core.Import;
using NLog;
using SudokuUI.Dialogs.ImageImport;

namespace SudokuUI.Services;

public class ImageImportService
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private PuzzleImporter? importer;

    // Fields
    // grid image
    // final puzzle?

    // Methods
    // Initialize
    // Cleanup

    public Task Initialize()
    {
        logger.Info("Image Import Service Initializing");

        if (importer is not null)
        {
            logger.Warn("Image Importer is already initialized. Skipping initialization.");
            return Task.CompletedTask;
        }

        return Task.Run(() =>
        {
            // Initialize the PuzzleImporter instance
            importer = new PuzzleImporter();
            logger.Info("Puzzle Importer initialized successfully.");
        });
    }

    public IEnumerable<ImageImportStep> Show()
    {
        string? import_method = null;

        // Logic to show the image import dialogs
        logger.Info("Image import Started");

        // Show the dialog to select an image file OR capture from camera
        var vm = new ImageImportDialogViewModel();
        var view = new ImageImportDialogView { DataContext = vm };
        yield return new ImageImportStep(vm, view, "Image Import");
        
        import_method = vm.DialogResult.Result;
        if (import_method is null)
        {
            logger.Info("Image import cancelled by user.");
            yield break; // User cancelled the dialog
        }

        // Dependending on the selected option, either process the image file or capture from camera
        logger.Info($"Import method selected: {import_method}");
        if (import_method == ImageImportDialogViewModel.SelectImage)
        {
            var select_vm = new SelectImageImportDialogViewModel();
            var select_view = new SelectImageImportDialogView { DataContext = select_vm };
            yield return new ImageImportStep(select_vm, select_view, "Select Image");
        }
        else
        {
            logger.Info("Capturing image from camera is not implemented yet.");
            yield break; // User cancelled the dialog
        }

        // Verify the processed image and the extracted Sudoku grid
        // VerifyImageImportDialogViewModel
    }
}
