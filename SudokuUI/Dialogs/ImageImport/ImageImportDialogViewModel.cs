using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NLog;
using OpenCvSharp;

namespace SudokuUI.Dialogs.ImageImport;

public partial class ImageImportDialogViewModel : ObservableObject, IDialogViewModel<ImportStepOutput>
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    
    private readonly TaskCompletionSource<ImportStepOutput> task_completion_source;

    public Task<ImportStepOutput> DialogResult => task_completion_source.Task;

    public ImageImportDialogViewModel()
    {
        task_completion_source = new TaskCompletionSource<ImportStepOutput>();
    }

    [RelayCommand]
    private void Select()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            DefaultExt = ".jpg",
            Filter = "Image Files|*.jpg;*.png",
            CheckFileExists = true
        };

        bool? result = dialog.ShowDialog();
        if (result == true)
        {
            logger.Info($"Selected image file: {dialog.FileName}");
            task_completion_source.SetResult(ImportStepOutput.SelectImage(dialog.FileName));
        }
    }

    [RelayCommand]
    private void Capture()
    {
        task_completion_source.SetResult(ImportStepOutput.CaptureImage());
    }

    [RelayCommand]
    private void Cancel()
    {
        task_completion_source.SetResult(ImportStepOutput.Cancel());
    }
}
