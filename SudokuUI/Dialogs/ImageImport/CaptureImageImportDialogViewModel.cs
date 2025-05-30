using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SudokuUI.Dialogs.ImageImport;

public partial class CaptureImageImportDialogViewModel : ObservableObject, IDialogViewModel<string?>
{
    private readonly TaskCompletionSource<string?> task_completion_source;

    public Task<string?> DialogResult => task_completion_source.Task;

    public CaptureImageImportDialogViewModel()
    {
        task_completion_source = new TaskCompletionSource<string?>();
    }

    [RelayCommand]
    private void Next()
    {
        task_completion_source.SetResult("next");
    }

    [RelayCommand]
    private void Cancel()
    {
        task_completion_source.SetResult(null); // Return null to indicate cancellation
    }
}
