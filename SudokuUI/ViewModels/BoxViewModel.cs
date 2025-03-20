using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Extensions;
using Core.Models;

namespace SudokuUI.ViewModels;

public partial class BoxViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<CellViewModel> cells = null!;
    
    [ObservableProperty]
    private int index = 0;

    public BoxViewModel(Unit box)
    {
        Cells = box.Cells.Select(c => new CellViewModel(c)).ToObservableCollection();
        Index = box.Index;
    }
}
