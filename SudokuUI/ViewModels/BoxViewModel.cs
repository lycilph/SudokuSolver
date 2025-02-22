using CommunityToolkit.Mvvm.ComponentModel;
using Core.Extensions;
using Core.Model;
using System.Collections.ObjectModel;

namespace SudokuUI.ViewModels;

public partial class BoxViewModel : ObservableObject
{
    private Unit box;

    [ObservableProperty]
    private ObservableCollection<CellViewModel> _cells;

    [ObservableProperty]
    private int _index = 0;

    public BoxViewModel(Unit box)
    {
        this.box = box;

        _cells = box.Cells.Select(c => new CellViewModel(c)).ToObservableCollection();
        _index = box.Index;
    }
}
