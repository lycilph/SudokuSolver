using CommunityToolkit.Mvvm.ComponentModel;
using Core.Extensions;
using Core.Model;
using SudokuUI.Services;
using System.Collections.ObjectModel;

namespace SudokuUI.ViewModels;

public partial class GridViewModel : ObservableObject
{
    private Grid grid;

    [ObservableProperty]
    private ObservableCollection<BoxViewModel> boxes;

    public GridViewModel(Grid grid, SelectionService selection_service)
    {
        this.grid = grid;

        boxes = grid.Boxes.Select(b => new BoxViewModel(b, selection_service)).ToObservableCollection();
    }
}
