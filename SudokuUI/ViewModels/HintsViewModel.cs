using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Commands;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class HintsViewModel : ObservableObject
{
    private TaskCompletionSource task_completion_source = new();

    private readonly SolverService solver_service;

    public Task Task => task_completion_source.Task;

    [ObservableProperty]
    private bool isOpen = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ApplyCommand))]
    [NotifyCanExecuteChangedFor(nameof(ApplyAndNextCommand))]
    private BaseCommand? command = null;

    private bool CanApply => Command != null;
    private bool CanApplyAndNext => Command != null;

    public HintsViewModel(SolverService solver_service)
    {
        this.solver_service = solver_service;
    }

    public void Show()
    {
        task_completion_source = new TaskCompletionSource();
        IsOpen = true;

        Command = solver_service.NextCommand();
        if (Command != null )
            solver_service.ShowVisualization(Command);
    }

    public void Hide() 
    { 
        solver_service.ClearVisualization();
        IsOpen = false; 
    }

    [RelayCommand(CanExecute = nameof(CanApply))]
    private void Apply()
    {
        if (Command != null)
            solver_service.Execute(Command);
        Cancel(); // This closes the overlay
    }

    [RelayCommand(CanExecute = nameof(CanApplyAndNext))]
    private void ApplyAndNext()
    {
        if (Command != null)
            solver_service.Execute(Command);

        Command = solver_service.NextCommand();
        if (Command != null)
            solver_service.ShowVisualization(Command);
    }

    [RelayCommand]
    public void Cancel() => task_completion_source.SetResult();
}
