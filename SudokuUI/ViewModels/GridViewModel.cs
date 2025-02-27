using CommunityToolkit.Mvvm.ComponentModel;
using Core.Extensions;
using Core.Model;
using SudokuUI.Controllers;
using System.Collections.ObjectModel;

namespace SudokuUI.ViewModels;

public partial class GridViewModel : ObservableObject
{
    private Grid grid;

    [ObservableProperty]
    private ObservableCollection<BoxViewModel> boxes;

    public GridViewModel(Grid grid, SelectionController selection_controller)
    {
        this.grid = grid;

        boxes = grid.Boxes.Select(b => new BoxViewModel(b, selection_controller)).ToObservableCollection();
    }
}
