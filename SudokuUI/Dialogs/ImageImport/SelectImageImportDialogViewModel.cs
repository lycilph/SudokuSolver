using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenCvSharp;

namespace SudokuUI.Dialogs.ImageImport;

public partial class SelectImageImportDialogViewModel : ObservableObject, IDialogViewModel<ImportStepOutput>
{
    private readonly TaskCompletionSource<ImportStepOutput> task_completion_source;

    [ObservableProperty]
    private string filename = string.Empty;

    [ObservableProperty]
    private Mat image;

    public Task<ImportStepOutput> DialogResult => task_completion_source.Task;

    public SelectImageImportDialogViewModel(string filename)
    {
        task_completion_source = new TaskCompletionSource<ImportStepOutput>();
        Filename = filename;
        Image = Cv2.ImRead(filename, ImreadModes.Color);
    }

    partial void OnImageChanging(Mat value)
    {
        Image?.Dispose(); // Dispose of the previous image to free resources
    }

    [RelayCommand]
    private void Next()
    {
        task_completion_source.SetResult(ImportStepOutput.Next(Image));
    }

    [RelayCommand]
    private void Cancel()
    {
        task_completion_source.SetResult(ImportStepOutput.Cancel());
        Image?.Dispose();
    }
}
