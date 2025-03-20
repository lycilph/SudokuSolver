using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Model.Actions;

namespace SudokuUI.ViewModels.Dialogs;

public partial class SolverHintDialogViewModel : ObservableObject
{
    private readonly TaskCompletionSource<bool> _taskCompletionSource;
    private readonly BaseSolveAction? _action;

    [ObservableProperty]
    private string _description = string.Empty;

    public Task<bool> DialogResult => _taskCompletionSource.Task;

    public SolverHintDialogViewModel(BaseSolveAction? action)
    {
        _action = action;
        _taskCompletionSource = new TaskCompletionSource<bool>();

        FormatDescription();
    }

    private void FormatDescription()
    {
        Description = _action?.ToString() ?? "No hints found...";
    }

    private bool CanApply() => _action != null;

    [RelayCommand(CanExecute = nameof(CanApply))]
    private void Apply()
    {
        _taskCompletionSource.SetResult(true);
    }

    [RelayCommand]
    private void Cancel()
    {
        _taskCompletionSource.SetResult(false);
    }
}
