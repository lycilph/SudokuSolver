using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.DancingLinks;
using Core.Engine;
using Core.Extensions;
using Core.Models;

namespace SudokuUI.Dialogs;

public partial class SolutionInformationDialogViewModel : ObservableObject, IDialogViewModel<bool>
{
    private readonly TaskCompletionSource<bool> task_completion_source;

    public Task<bool> DialogResult => task_completion_source.Task;

    [ObservableProperty]
    private string puzzle;

    [ObservableProperty]
    private int solutionCount;

    [ObservableProperty]
    private long elapsedTime;

    [ObservableProperty]
    private Grade grade;

    [ObservableProperty]
    private string gradeName;

    public SolutionInformationDialogViewModel(string source)
    {
        task_completion_source = new TaskCompletionSource<bool>();

        var grid = new Grid().Load(source,true);
        Puzzle = grid.ToSimpleString();

        (var solutions, var stats) = DancingLinksSolver.Solve(grid, true);
        SolutionCount = solutions.Count;
        ElapsedTime = stats.ElapsedTime;

        // Grade here
        grade = Grader.Grade(grid);
        GradeName = Difficulty.GetDifficultyName(grade.Difficulty);
    }

    [RelayCommand]
    private void Ok()
    {
        task_completion_source.SetResult(true);
    }
}
