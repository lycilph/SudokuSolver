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

    [ObservableProperty]
    private bool isActive = false;

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

    public Task<VictoryResult> Activate(TimeSpan time)
    {
        task_completion_source = new TaskCompletionSource<VictoryResult>();
        Elapsed = time;
        IsActive = true; 

        return task_completion_source.Task;
    }

    public void AddStatistics(Statistics stats)
    {
        Statistics = stats;
        ShowStatistics = true;
    }

    private void Complete(VictoryResult result) 
    {
        task_completion_source.SetResult(result);

        ShowStatistics = false;
        Statistics.Reset();

        IsActive = false;
    }

    [RelayCommand]
    private void New() => Complete(new VictoryResult(VictoryResult.ResultType.NewGame));

    [RelayCommand]
    private void Clear() => Complete(new VictoryResult(VictoryResult.ResultType.Clear));

    [RelayCommand]
    private void Reset() => Complete(new VictoryResult(VictoryResult.ResultType.Reset));
}