using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace SudokuUI.Dialogs;

public partial class ExportDialogViewModel : ObservableObject, IDialogViewModel<bool>
{
    private readonly TaskCompletionSource<bool> task_completion_source;

    [ObservableProperty]
    private string puzzle = string.Empty;

    public Task<bool> DialogResult => task_completion_source.Task;

    public ExportDialogViewModel()
    {
        task_completion_source = new TaskCompletionSource<bool>();
    }

    [RelayCommand]
    private void Ok()
    {
        task_completion_source.SetResult(true);
    }

    [RelayCommand]
    private void CopyToClipboard()
    {
        Clipboard.SetText(Puzzle);
    }
}
