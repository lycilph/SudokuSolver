using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Commands;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class HintsViewModel : ObservableObject
{
    private TaskCompletionSource task_completion_source = new();

    private readonly SolverService solver_service;

    private bool showing_elements;
    private int element_index;

    [ObservableProperty]
    private bool isActive = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ApplyCommand))]
    [NotifyCanExecuteChangedFor(nameof(ApplyAndNextCommand))]
    [NotifyCanExecuteChangedFor(nameof(ResetCommand))]
    [NotifyCanExecuteChangedFor(nameof(PreviousCommandElementCommand))]
    [NotifyCanExecuteChangedFor(nameof(NextCommandElementCommand))]
    private BaseCommand? command = null;

    [ObservableProperty]
    private string description = string.Empty;

    private bool HasCommand => Command != null;
    private bool HasCommandElements => Command?.Elements.Count > 1;

    public HintsViewModel(SolverService solver_service)
    {
        this.solver_service = solver_service;
    }

    public Task Activate(BaseCommand? cmd)
    {
        task_completion_source = new TaskCompletionSource();
        IsActive = true;

        Command = cmd ?? solver_service.NextCommand();
        if (Command != null)
            solver_service.ShowVisualization(Command);

        showing_elements = false;
        UpdateDescription();

        return task_completion_source.Task;
    }

    private void Complete()
    {
        task_completion_source.SetResult();
        solver_service.ClearVisualization();
        IsActive = false;
    }

    private void UpdateDescription()
    {
        if (showing_elements)
            Description = Command?.Elements[element_index].Description ?? "No element found";
        else
            Description = Command?.Description ?? "No hints found";
    }

    [RelayCommand(CanExecute = nameof(HasCommandElements))]
    private void Reset()
    {
        if (Command == null)
            return;

        solver_service.ShowVisualization(Command);
        showing_elements = false;
        UpdateDescription();
    }

    [RelayCommand(CanExecute = nameof(HasCommandElements))]
    private void PreviousCommandElement()
    {
        if (Command == null)
            return;

        if (!showing_elements)
        {
            showing_elements = true;
            element_index = 0;
        }
        else
        {
            element_index--;
            if (element_index < 0)
                element_index = Command.Elements.Count - 1;
        }

        solver_service.ShowVisualization(Command, element_index);
        UpdateDescription();
    }

    [RelayCommand(CanExecute = nameof(HasCommandElements))]
    private void NextCommandElement()
    {
        if (Command == null)
            return;

        if (!showing_elements)
        {
            showing_elements = true;
            element_index = 0;
        }
        else
        {
            element_index++;
            if (element_index >= Command.Elements.Count)
                element_index = 0;
        }

        solver_service.ShowVisualization(Command, element_index);
        UpdateDescription();
    }

    [RelayCommand(CanExecute = nameof(HasCommand))]
    private void Apply()
    {
        if (Command != null)
            solver_service.Execute(Command);

        // This might have triggered a puzzle solved event (which cancels this), so check if we are already completed
        if (!task_completion_source.Task.IsCompleted)
            Complete();
    }

    [RelayCommand(CanExecute = nameof(HasCommand))]
    private void ApplyAndNext()
    {
        if (Command != null)
            solver_service.Execute(Command);

        Command = solver_service.NextCommand();
        if (Command != null)
            solver_service.ShowVisualization(Command);

        showing_elements = false;
        UpdateDescription();
    }

    [RelayCommand]
    public void Cancel()
    {
        Complete();
    }
}
