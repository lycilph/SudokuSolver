using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Extensions;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class GridViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<BoxViewModel> boxes = null!;

    public GridViewModel(PuzzleService puzzle_service)
    {
        Boxes = puzzle_service.Grid.Boxes.Select(b => new BoxViewModel(b)).ToObservableCollection();
    }
}
