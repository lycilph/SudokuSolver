using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SudokuUI.Extensions;
using System.Collections.ObjectModel;

namespace SudokuUI.ViewModels;

public partial class SelectionViewModel : ObservableObject
{
    public ObservableCollection<DigitSelectionViewModel> DigitSelections { get; private set; }

    public SelectionViewModel()
    {
        DigitSelections = [.. Enumerable.Range(1, 9).Select(d => new DigitSelectionViewModel(d, Select))];
    }

    [RelayCommand]
    private void ClearSelection()
    {
        DigitSelections.ForEach(s => s.Selected = false);
    }

    private void Select(int digit)
    {
        DigitSelections.ForEach(s => s.Selected = (digit == s.Digit));
    }
}
