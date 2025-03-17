using CommunityToolkit.Mvvm.ComponentModel;
using Core.Infrastructure;
using System.Collections.ObjectModel;
using VisualStrategyDebugger.Service;

namespace VisualStrategyDebugger.ViewModels;

public partial class GridViewModel : ObservableObject
{
    private readonly GridService grid_service;

    [ObservableProperty]
    private ObservableCollection<CellViewModel> cells;

    public GridViewModel(GridService grid_service)
    {
        this.grid_service = grid_service;

        Cells = grid_service.Grid.Cells.Select(c => new CellViewModel(c)).ToObservableCollection();
    }
}
