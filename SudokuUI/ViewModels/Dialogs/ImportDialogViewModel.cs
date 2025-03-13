using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SudokuUI.ViewModels.Dialogs;

public partial class ImportDialogViewModel : ObservableObject
{
    private readonly TaskCompletionSource<string?> _taskCompletionSource;

    [ObservableProperty]
    private string puzzle = string.Empty;

    public Task<string?> DialogResult => _taskCompletionSource.Task;

    public ImportDialogViewModel()
    {
        _taskCompletionSource = new TaskCompletionSource<string?>();
    }

    [RelayCommand]
    private void Ok()
    {
        _taskCompletionSource.SetResult(Puzzle); // Return the input text as the result   
    }

    [RelayCommand]
    private void Cancel()
    {
        _taskCompletionSource.SetResult(null); // Return null to indicate cancellation
    }
}
