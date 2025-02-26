using CommunityToolkit.Mvvm.ComponentModel;

namespace SudokuUI.ViewModels;

public partial class DigitSelectionViewModel : ObservableObject
{
    [ObservableProperty]
    private int _digit;

    // The # of this digit left in the puzzle (ie. how many is left to place)
    [ObservableProperty]
    private int _missing = 9;

    [ObservableProperty]
    private bool _selected = false;

    public DigitSelectionViewModel(int digit)
    {
        Digit = digit;
    }
}
