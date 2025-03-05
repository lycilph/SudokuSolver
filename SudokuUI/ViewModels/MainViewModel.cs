using CommunityToolkit.Mvvm.ComponentModel;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private GridViewModel grid = null!;

    public MainViewModel(PuzzleService puzzle_service)
    {
        Grid = new GridViewModel(puzzle_service.Grid);
    }
}