using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using Core.Commands;
using SudokuUI.Services;
using SudokuUI.Visualizers;

namespace SudokuUI.ViewModels;

public partial class SolverOverlayViewModel : ObservableObject
{
    private readonly PuzzleService puzzle_service;
    private readonly SolverService solver_service;
    private readonly HighlightService highlight_service;
    private readonly UndoRedoService undo_service;
    private readonly GridViewModel gridVM;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ApplyCommand))]
    [NotifyCanExecuteChangedFor(nameof(ApplyAndNextCommand))]
    private ICommand? command = null;

    [ObservableProperty]
    private bool isOpen = false;

    [ObservableProperty]
    private string description = string.Empty;

    private bool CanApply => Command != null;
    private bool CanApplyAndNext => Command != null;

    public SolverOverlayViewModel(PuzzleService puzzle_service,
                                  SolverService solver_service,
                                  HighlightService highlight_service,
                                  UndoRedoService undo_service,
                                  GridViewModel gridVM)
    {
        this.puzzle_service = puzzle_service;
        this.solver_service = solver_service;
        this.highlight_service = highlight_service;
        this.undo_service = undo_service;
        this.gridVM = gridVM;
    }

    partial void OnIsOpenChanged(bool value)
    {
        if (isOpen)
            ShowVisualization();
        else
            // Clear visualization here
            highlight_service.Clear();
    }

    public void ShowVisualization()
    {
        if (Command != null && Command is BaseCommand base_command)
        {
            var type = Command.GetType();
            var visualizer = solver_service.Visualizers[type];

            // Show visualization here
            visualizer.Show(gridVM, base_command);
        }
    }

    public bool NextHint()
    {
        Command = Solver.Step(puzzle_service.Grid);
        Description = Command?.Description ?? "No more hints found";

        return Command != null;
    }
    
    [RelayCommand(CanExecute = nameof(CanApply))]
    private void Apply()
    {
        if (Command != null)
            undo_service.Execute(Command);
        Close();
    }

    [RelayCommand(CanExecute = nameof(CanApplyAndNext))]
    private void ApplyAndNext()
    {
        if (Command != null)
            undo_service.Execute(Command);
        NextHint();
        ShowVisualization();
    }

    [RelayCommand]
    public void Close() => IsOpen = false;
    
    public void Show() => IsOpen = true;
    public void Toggle() => IsOpen = !IsOpen;
}
