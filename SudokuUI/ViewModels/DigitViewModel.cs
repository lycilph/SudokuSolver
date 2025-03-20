using CommunityToolkit.Mvvm.ComponentModel;

namespace SudokuUI.ViewModels;

public partial class DigitViewModel : ObservableObject
{
    [ObservableProperty]
    private int digit;

    // The # of this digit left in the puzzle (ie. how many is left to place)
    [ObservableProperty]
    private int missing = 9;

    [ObservableProperty]
    private bool selected = false;

    public DigitViewModel(int digit)
    {
        Digit = digit;
    }
}
