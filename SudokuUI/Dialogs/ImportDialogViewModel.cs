using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SudokuUI.Dialogs;

public partial class ImportDialogViewModel : ObservableObject, IDialogViewModel<string?>
{
    private readonly TaskCompletionSource<string?> task_completion_source;

    [ObservableProperty]
    private string puzzle = string.Empty;

    public Task<string?> DialogResult => task_completion_source.Task;

    public ImportDialogViewModel()
    {
        task_completion_source = new TaskCompletionSource<string?>();
    }

    [RelayCommand]
    private void Ok()
    {
        task_completion_source.SetResult(Puzzle); // Return the input text as the result   
    }

    [RelayCommand]
    private void Cancel()
    {
        task_completion_source.SetResult(null); // Return null to indicate cancellation
    }
}
