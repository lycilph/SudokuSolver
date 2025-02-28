using CommunityToolkit.Mvvm.ComponentModel;
using Core.Extensions;
using Core.Model;
using SudokuUI.Services;
using System.Collections.ObjectModel;

namespace SudokuUI.ViewModels;

public partial class BoxViewModel : ObservableObject
{
    private Unit box;

    [ObservableProperty]
    private ObservableCollection<CellViewModel> _cells;

    [ObservableProperty]
    private int _index = 0;

    public BoxViewModel(Unit box, SelectionService selection_service)
    {
        this.box = box;

        _cells = box.Cells.Select(c => new CellViewModel(c, selection_service)).ToObservableCollection();
        _index = box.Index;
    }
}
