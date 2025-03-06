using CommunityToolkit.Mvvm.ComponentModel;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private GridViewModel _grid = null!;

    [ObservableProperty]
    private SelectionViewModel _selection;

    public MainViewModel(PuzzleService puzzle_service, SelectionViewModel selection)
    {
        Grid = new GridViewModel(puzzle_service.Grid);
        Selection = selection;
    }
}