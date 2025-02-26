using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace SudokuUI.ViewModels;

public partial class SelectionViewModel : ObservableObject
{
    public ObservableCollection<DigitSelectionViewModel> DigitSelections { get; private set; }

    public SelectionViewModel()
    {
        DigitSelections = [.. Enumerable.Range(1, 9).Select(d => new DigitSelectionViewModel(d))];
    }
}
