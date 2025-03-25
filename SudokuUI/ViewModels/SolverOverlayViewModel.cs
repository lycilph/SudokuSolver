using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class SolverOverlayViewModel : ObservableObject
{
    private readonly SolverService solver_service;

    public bool IsOpen
    {
        get => solver_service.IsShown;
        set => solver_service.IsShown = value;
    }

    [ObservableProperty]
    private string description = string.Empty;

    private bool CanApply => solver_service.Command != null;
    private bool CanApplyAndNext => solver_service.Command != null;

    public SolverOverlayViewModel(SolverService solver_service)
    {
        this.solver_service = solver_service;

        solver_service.PropertyChanged += (s, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(SolverService.IsShown):
                    OnPropertyChanged(nameof(IsOpen));
                    Update();
                    break;
                case nameof(SolverService.Command):
                    ApplyCommand.NotifyCanExecuteChanged();
                    ApplyAndNextCommand.NotifyCanExecuteChanged();
                    Update();
                    break;
            };
        };
    }

    private void Update()
    {
        if (IsOpen)
        {
            Description = solver_service.Command?.Description ?? "No more hints found";
            solver_service.ShowVisualization();
        }
        else
            solver_service.ClearVisualization();
    }

    [RelayCommand(CanExecute = nameof(CanApply))]
    private void Apply()
    {
        solver_service.ExecuteCommand();
        solver_service.Hide();
    }

    [RelayCommand(CanExecute = nameof(CanApplyAndNext))]
    private void ApplyAndNext()
    {
        solver_service.ExecuteCommand();
        solver_service.NextHint();
    }

    [RelayCommand]
    public void Close() => solver_service.Hide();
}
