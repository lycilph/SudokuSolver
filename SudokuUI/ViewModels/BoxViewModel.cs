using CommunityToolkit.Mvvm.ComponentModel;
using Core.Extensions;
using Core.Model;
using System.Collections.ObjectModel;

namespace SudokuUI.ViewModels;

public partial class BoxViewModel : ObservableObject
{
    private Unit box;

    [ObservableProperty]
    private ObservableCollection<CellViewModel> cells;

    public BoxViewModel(Unit box)
    {
        this.box = box;

        cells = box.Cells.Select(c => new CellViewModel(c)).ToObservableCollection();
    }
}
