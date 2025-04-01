using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Engine;
using Core.Extensions;
using Core.Models;
using SudokuUI.Infrastructure;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class VictoryViewModel : ObservableObject
{
    private TaskCompletionSource<VictoryResult> task_completion_source = new();
    //public event EventHandler RequestNewGame = null!;
    //public event EventHandler RequestClearGame = null!;
    //public event EventHandler RequestRestartGame = null!;

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

    public Task<VictoryResult> Task => task_completion_source.Task;

    public VictoryViewModel(PuzzleService puzzle_service)
    {
        Cells = puzzle_service.Grid.Cells.ToObservableCollection();
    }

    public void Show(TimeSpan time)
    {
        task_completion_source = new TaskCompletionSource<VictoryResult>();
        Elapsed = time;
        IsOpen = true; 
    }

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
    private void New() => task_completion_source.SetResult(new VictoryResult(VictoryResult.ResultType.NewGame));

    [RelayCommand]
    private void Clear() => task_completion_source.SetResult(new VictoryResult(VictoryResult.ResultType.Clear));

    [RelayCommand]
    private void Reset() => task_completion_source.SetResult(new VictoryResult(VictoryResult.ResultType.Reset));
}