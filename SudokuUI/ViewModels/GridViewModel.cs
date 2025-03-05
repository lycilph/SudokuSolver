using CommunityToolkit.Mvvm.ComponentModel;
using Core.Infrastructure;
using Core.Model;
using System.Collections.ObjectModel;

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
