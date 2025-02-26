using CommunityToolkit.Mvvm.ComponentModel;
using Core.Model;
using Core.Strategies;

namespace SudokuUI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly Puzzle puzzle = new("4......38.32.941...953..24.37.6.9..4.29..16.36.47.3.9.957..83....39..4..24..3.7.9");

    [ObservableProperty]
    private GridViewModel _grid;

    [ObservableProperty]
    private SelectionViewModel _selections;

    public MainViewModel()
    {
        BasicEliminationStrategy.ExecuteAndApply(puzzle.Grid);
        _grid = new GridViewModel(puzzle.Grid);
        _selections = new SelectionViewModel();
    }
}
