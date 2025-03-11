using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace SudokuUI.ViewModels;

public partial class ExportDialogViewModel : ObservableObject
{
    private readonly TaskCompletionSource _taskCompletionSource;

    [ObservableProperty]
    private string puzzle = string.Empty;

    public Task DialogResult => _taskCompletionSource.Task;

    public ExportDialogViewModel()
    {
        _taskCompletionSource = new TaskCompletionSource();
    }

    [RelayCommand]
    private void Ok()
    {
        _taskCompletionSource.SetResult();
    }

    [RelayCommand]
    private void CopyToClipboard()
    {
        Clipboard.SetText(Puzzle);
    }
}
