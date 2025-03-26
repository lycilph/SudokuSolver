using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Extensions;
using Core.Models;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class VictoryOverlayViewModel : ObservableObject
{
    private readonly PuzzleService puzzle_service;

    public event EventHandler RequestNewGame = null!;
    public event EventHandler RequestClearGame = null!;

    [ObservableProperty]
    private bool isOpen = false;

    [ObservableProperty]
    public ObservableCollection<Cell> cells;

    [ObservableProperty]
    private TimeSpan elapsed = TimeSpan.FromSeconds(0);

    public VictoryOverlayViewModel(PuzzleService puzzle_service)
    {
        this.puzzle_service = puzzle_service;

        Cells = puzzle_service.Grid.Cells.ToObservableCollection();
    }

    public void Show() => IsOpen = true;
    public void Close() => IsOpen = false;
    public void Toggle() => IsOpen = !IsOpen;

    [RelayCommand]
    private void New()
    {
        Close();
        RequestNewGame?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Clear()
    {
        Close();
        RequestClearGame?.Invoke(this, EventArgs.Empty);
    }
}