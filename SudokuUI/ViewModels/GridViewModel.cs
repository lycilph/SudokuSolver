using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Extensions;
using Core.Models;

namespace SudokuUI.ViewModels;

public partial class GridViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<BoxViewModel> boxes = null!;

    public GridViewModel(Grid grid)
    {
        Boxes = grid.Boxes.Select(b => new BoxViewModel(b)).ToObservableCollection();
    }
}
