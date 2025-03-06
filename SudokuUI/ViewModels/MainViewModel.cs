using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly SelectionService selection_service;

    [ObservableProperty]
    private GridViewModel _grid = null!;

    [ObservableProperty]
    private SelectionViewModel _selection;

    public MainViewModel(PuzzleService puzzle_service, SelectionService selection_service, SelectionViewModel selection)
    {
        this.selection_service = selection_service;

        Grid = new GridViewModel(puzzle_service.Grid);
        Selection = selection;
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
}