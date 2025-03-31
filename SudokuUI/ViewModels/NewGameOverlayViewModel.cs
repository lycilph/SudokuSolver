using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Engine;

namespace SudokuUI.ViewModels;

public partial class NewGameOverlayViewModel : ObservableObject
{
    private TaskCompletionSource<Difficulty?> task_completion_source = new();

    [ObservableProperty]
    private bool isOpen = false;

    public Task<Difficulty?> Task => task_completion_source.Task;

    public void Show()
    {
        task_completion_source = new TaskCompletionSource<Difficulty?>();
        IsOpen = true;
    }

    public void Close()
    {
        IsOpen = false;
        task_completion_source.SetResult(null);
    }

    [RelayCommand]
    private void Easy()
    {
        IsOpen = false;
        task_completion_source.SetResult(Difficulty.Easy());
    }

    [RelayCommand]
    private void Medium()
    {
        IsOpen = false;
        task_completion_source.SetResult(Difficulty.Medium());
    }

    [RelayCommand]
    private void Hard()
    {
        IsOpen = false;
        task_completion_source.SetResult(Difficulty.Hard());
    }

    [RelayCommand]
    private void VeryHard()
    {
        IsOpen = false;
        task_completion_source.SetResult(Difficulty.VeryHard());
    }
}
