using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.Commands;
using SudokuUI.Messages;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class HintsViewModel : ObservableObject
{
    private TaskCompletionSource task_completion_source = new();

    private readonly SolverService solver_service;

    [ObservableProperty]
    private bool isActive = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ApplyCommand))]
    [NotifyCanExecuteChangedFor(nameof(ApplyAndNextCommand))]
    [NotifyCanExecuteChangedFor(nameof(ResetCommand))]
    [NotifyCanExecuteChangedFor(nameof(PreviousCommandElementCommand))]
    [NotifyCanExecuteChangedFor(nameof(NextCommandElementCommand))]
    private BaseCommand? command = null;

    private bool CanApply => Command != null;
    private bool CanApplyAndNext => Command != null;

    private bool CanChangeCommandElements => Command?.Elements.Count > 1;

    public HintsViewModel(SolverService solver_service)
    {
        this.solver_service = solver_service;
    }

    public Task Activate(BaseCommand? cmd)
    {
        task_completion_source = new TaskCompletionSource();
        IsActive = true;

        Command = cmd ?? solver_service.NextCommand();
        if (Command != null )
            solver_service.ShowVisualization(Command);

        return task_completion_source.Task;
    }

    private void Complete() 
    { 
        task_completion_source.SetResult();
        solver_service.ClearVisualization();
        IsActive = false; 
    }

    [RelayCommand(CanExecute = nameof(CanChangeCommandElements))]
    private void Reset()
    {
        WeakReferenceMessenger.Default.Send(new ShowNotificationMessage("Reset command"));
    }

    [RelayCommand(CanExecute = nameof(CanChangeCommandElements))]
    private void PreviousCommandElement()
    {
        WeakReferenceMessenger.Default.Send(new ShowNotificationMessage("Previous Command Element"));
    }

    [RelayCommand(CanExecute = nameof(CanChangeCommandElements))]
    private void NextCommandElement()
    {
        WeakReferenceMessenger.Default.Send(new ShowNotificationMessage("Next Command Element"));
    }

    [RelayCommand(CanExecute = nameof(CanApply))]
    private void Apply()
    {
        if (Command != null)
            solver_service.Execute(Command);

        // This might have triggered a puzzle solved event (which cancels this), so check if we are already completed
        if (!task_completion_source.Task.IsCompleted)
            Complete();
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
    public void Cancel()
    {
        Complete();
    }
}
