using CommunityToolkit.Mvvm.ComponentModel;
using Core.Infrastructure;
using SudokuUI.Services;
using System.Collections.ObjectModel;

namespace SudokuUI.ViewModels;

public partial class GridViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<BoxViewModel> boxes = null!;

    public GridViewModel(PuzzleService puzzle_service)
    {
        var grid = puzzle_service.Grid;

        Boxes = grid.Boxes.Select(b => new BoxViewModel(b)).ToObservableCollection();
    }
}
