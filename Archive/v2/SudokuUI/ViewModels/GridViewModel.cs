using CommunityToolkit.Mvvm.ComponentModel;
using Core.Extensions;
using Core.Model;
using System.Collections.ObjectModel;

namespace SudokuUI.ViewModels;

public partial class GridViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<BoxViewModel> boxes;

    public GridViewModel(Grid grid)
    {
        boxes = grid.Boxes.Select(b => new BoxViewModel(b)).ToObservableCollection();
    }
}
