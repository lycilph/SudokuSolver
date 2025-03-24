using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class SolverOverlayViewModel : ObservableObject
{
    private readonly PuzzleService puzzle_service;

    [ObservableProperty]
    private bool isOpen = false;

    [ObservableProperty]
    private string description = string.Empty;

    public SolverOverlayViewModel(PuzzleService puzzle_service)
    {
        this.puzzle_service = puzzle_service;
    }

    partial void OnIsOpenChanged(bool value)
    {
        if (IsOpen)
        {
            var cmd = Solver.Step(puzzle_service.Grid);
            Description = cmd?.Description ?? "No more steps to take";
        }
    }

    [RelayCommand]
    public void Close() => IsOpen = false;
    
    public void Show() => IsOpen = true;
    public void Toggle() => IsOpen = !IsOpen;
}
