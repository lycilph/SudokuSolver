using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core;
using Core.DancingLinks;
using Core.Model;
using MahApps.Metro.Controls.Dialogs;
using SudokuUI.Services;
using SudokuUI.Views;
using System.Text;

namespace SudokuUI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly SelectionService selection_service;

    [ObservableProperty]
    private PuzzleService puzzleService;

    [ObservableProperty]
    private GridViewModel _grid = null!;

    [ObservableProperty]
    private SelectionViewModel _selection;

    [ObservableProperty]
    private SettingsViewModel _settings;

    public MainViewModel(SelectionService selection_service, PuzzleService puzzle_service, GridViewModel grid, SelectionViewModel selection, SettingsViewModel settings)
    {
        this.selection_service = selection_service;

        PuzzleService = puzzle_service;
        Grid = grid;
        Selection = selection;
        Settings = settings;
    }


    [RelayCommand]
    private void NewPuzzle()
    {
        var temp = Generator.Generate();
        var str = temp.Grid.ToSimpleString();

        PuzzleService.Import(str);
    }

    [RelayCommand]
    private void Clear()
    {
        PuzzleService.Clear();
    }

    [RelayCommand]
    private async Task Import()
    {
        var vm = new ImportDialogViewModel();
        var view = new ImportDialogView { DataContext = vm };
        var dialog = new CustomDialog { Title = "Import Puzzle", Content = view };
        
        await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
        var result = await vm.DialogResult;
        await DialogCoordinator.Instance.HideMetroDialogAsync(this, dialog);

        if (!string.IsNullOrWhiteSpace(result))
            PuzzleService.Import(result);
    }

    [RelayCommand]
    private async Task Export()
    {
        var vm = new ExportDialogViewModel { Puzzle = PuzzleService.Export() };
        var view = new ExportDialogView { DataContext = vm };
        var dialog = new CustomDialog { Title = "Export Puzzle", Content = view };

        await DialogCoordinator.Instance.ShowMetroDialogAsync(this, dialog);
        await vm.DialogResult;
        await DialogCoordinator.Instance.HideMetroDialogAsync(this, dialog);
    }

    [RelayCommand]
    private async Task ShowSolutionCount()
    {
        var temp = new Puzzle(PuzzleService.Grid.ToSimpleString()); // Create a copy of the puzzle
        var count = DancingLinksSolver.Solve(temp).Count; // This can change the puzzle (thus the copy)

        var sb = new StringBuilder();
        sb.AppendLine($"The puzzle has {count} solutions");
        sb.AppendLine($"Execution Time: {temp.Stats.ElapsedTime} ms");
        await DialogCoordinator.Instance.ShowMessageAsync(this, "Solution Count", sb.ToString());
    }

    [RelayCommand]
    private void NumberKeyPressed(string number)
    {
        if (int.TryParse(number, out int digit))
            selection_service.Digit = digit;
    }

    [RelayCommand]
    private void ClearSelection()
    {
        selection_service.ClearDigit();
    }

    [RelayCommand]
    private void ToggleInputMode()
    {
        selection_service.ToggleInputMode();
    }

    [RelayCommand]
    private void ShowSettings()
    {
        Settings.IsOpen = true;
    }

    [RelayCommand]
    private void HideSettings()
    {
        Settings.IsOpen = false;
    }
}