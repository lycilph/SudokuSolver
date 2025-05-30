using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Extensions;
using Core.Models;
using OpenCvSharp;
using System.Collections.ObjectModel;

namespace SudokuUI.Dialogs.ImageImport;

public partial class VerifyImageImportDialogViewModel : ObservableObject, IDialogViewModel<ImportStepOutput>
{
    private readonly TaskCompletionSource<ImportStepOutput> task_completion_source;

    public Task<ImportStepOutput> DialogResult => task_completion_source.Task;
    
    [ObservableProperty]
    private Mat image;

    [ObservableProperty]
    public ObservableCollection<Cell> cells = [];

    [ObservableProperty]
    private List<NumberViewModel> numbers = [];

    [ObservableProperty]
    private int selectedNumber = 0;

    public VerifyImageImportDialogViewModel(Mat image, string puzzle_source)
    {
        Image = image;
        Cells = new Grid().Load(puzzle_source).Cells.ToObservableCollection();
        Numbers = [.. Enumerable.Range(1, 9).Select(n => new NumberViewModel(this, n))];
        Update(Numbers.First());

        task_completion_source = new TaskCompletionSource<ImportStepOutput>();
    }

    public void Update(NumberViewModel vm)
    {
        SelectedNumber = vm.Number;
        foreach (var number in Numbers)
            number.Selected = number == vm;
    }

    [RelayCommand]
    private void Set(Cell cell)
    {
        cell.Value = SelectedNumber;
    }

    [RelayCommand]
    private void Clear(Cell cell)
    {
        cell.Value = 0;
    }

    [RelayCommand]
    private void Done()
    {
        var puzzle = 
            string.Join("", Cells.Select(c => c.Value.ToString()))
                  .Replace("0", ".");

        task_completion_source.SetResult(ImportStepOutput.Done(puzzle));
    }

    [RelayCommand]
    private void Cancel()
    {
        task_completion_source.SetResult(ImportStepOutput.Cancel());
    }
}
