using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Model.Actions;
using System.Text;

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
        if (_action == null)
        {
            Description = "No hints found...";
        }
        else
        {
            var sb = new StringBuilder();
            sb.AppendLine(_action.Description);
            foreach (var elem in _action.Elements)
                sb.AppendLine($" * {elem.Description}");
            Description = sb.ToString();
        }
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
