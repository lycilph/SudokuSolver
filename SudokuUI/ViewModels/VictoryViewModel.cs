using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Engine;
using Core.Extensions;
using Core.Models;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class VictoryViewModel : ObservableObject
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

    public VictoryViewModel(PuzzleService puzzle_service)
    {
        Cells = puzzle_service.Grid.Cells.ToObservableCollection();
    }

    public void Show() => IsOpen = true;
    public void Hide() 
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
        Hide();
        RequestNewGame?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Clear()
    {
        Hide();
        RequestClearGame?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Restart()
    {
        Hide();
        RequestRestartGame?.Invoke(this, EventArgs.Empty);
    }
}