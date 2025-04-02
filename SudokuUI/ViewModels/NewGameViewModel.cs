using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Engine;

namespace SudokuUI.ViewModels;

public partial class NewGameViewModel : ObservableObject
{
    private TaskCompletionSource<Difficulty?> task_completion_source = new();

    [ObservableProperty]
    private bool isActive = false;
    
    public Task<Difficulty?> Activate()
    {
        task_completion_source = new TaskCompletionSource<Difficulty?>();
        IsActive = true;

        return task_completion_source.Task;
    }

    private void Complete(Difficulty? difficulty)
    {
        task_completion_source.SetResult(difficulty);
        IsActive = false;
    }

    public void Cancel() => Complete(null);

    [RelayCommand]
    private void Easy() => Complete(Difficulty.Easy());

    [RelayCommand]
    private void Medium() => Complete(Difficulty.Medium());

    [RelayCommand]
    private void Hard() => Complete(Difficulty.Hard());

    [RelayCommand]
    private void VeryHard() => Complete(Difficulty.VeryHard());
}
