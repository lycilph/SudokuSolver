using CommunityToolkit.Mvvm.ComponentModel;
using Core.Extensions;
using Core.Model;
using System.Collections.ObjectModel;

namespace SudokuUI.ViewModels;

public partial class CellViewModel : ObservableObject
{
    private Cell cell;

    [ObservableProperty]
    private ObservableCollection<HintViewModel> hints;

    [ObservableProperty]
    private int value = 0;

    public CellViewModel(Cell cell)
    {
        this.cell = cell;

        hints = Enumerable.Range(1, 9).Select(i => new HintViewModel(i)).ToObservableCollection();
    }
}
