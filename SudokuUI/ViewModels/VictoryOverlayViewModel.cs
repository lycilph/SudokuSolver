using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Engine;
using Core.Extensions;
using Core.Models;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class VictoryOverlayViewModel : ObservableObject
{
    public event EventHandler RequestNewGame = null!;
    public event EventHandler RequestClearGame = null!;
    public event EventHandler RequestRestartGame = null!;

    [ObservableProperty]
    private bool isOpen = false;

    [ObservableProperty]
    public ObservableCollection<Cell> cells;

    [ObservableProperty]
    private TimeSpan elapsed = TimeSpan.FromSeconds(0);

    [ObservableProperty]
    private Statistics statistics = new();

    [ObservableProperty]
    private bool showStatistics = false;

    public VictoryOverlayViewModel(PuzzleService puzzle_service)
    {
        Cells = puzzle_service.Grid.Cells.ToObservableCollection();
    }

    public void Show() => IsOpen = true;
    public void Close() 
    {
        ShowStatistics = false;
        Statistics.Reset();

        IsOpen = false;
    }

    public void AddStatistics(Statistics stats)
    {
        Statistics = stats;
        ShowStatistics = true;
    }

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

    [RelayCommand]
    private void Restart()
    {
        Close();
        RequestRestartGame?.Invoke(this, EventArgs.Empty);
    }
}