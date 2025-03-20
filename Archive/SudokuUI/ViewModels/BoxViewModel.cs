using CommunityToolkit.Mvvm.ComponentModel;
using Core.Infrastructure;
using Core.Model;
using System.Collections.ObjectModel;

namespace SudokuUI.ViewModels;

public partial class BoxViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<CellViewModel> _cells = null!;

    [ObservableProperty]
    private int _index = 0;

    public BoxViewModel(Unit box)
    {
        Cells = box.Cells.Select(c => new CellViewModel(c)).ToObservableCollection();
        Index = box.Index;
    }
}
