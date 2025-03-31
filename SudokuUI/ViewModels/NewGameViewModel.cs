using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Engine;

namespace SudokuUI.ViewModels;

public partial class NewGameViewModel : ObservableObject
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

    public void Hide() => IsOpen = false;

    public void Cancel() => task_completion_source.SetResult(null);

    [RelayCommand]
    private void Easy() => task_completion_source.SetResult(Difficulty.Easy());

    [RelayCommand]
    private void Medium() => task_completion_source.SetResult(Difficulty.Medium());

    [RelayCommand]
    private void Hard() => task_completion_source.SetResult(Difficulty.Hard());

    [RelayCommand]
    private void VeryHard() => task_completion_source.SetResult(Difficulty.VeryHard());
}
