using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SudokuUI.ViewModels;

public partial class DigitSelectionViewModel : ObservableObject
{
    private readonly Action<int> select_digit;

    [ObservableProperty]
    private int _digit;

    // The # of this digit left in the puzzle (ie. how many is left to place)
    [ObservableProperty]
    private int _missing = 9;

    [ObservableProperty]
    private bool _selected = false;

    public DigitSelectionViewModel(int digit, Action<int> select_action)
    {
        Digit = digit;
        select_digit = select_action;

        
    }

    [RelayCommand]
    private void Select()
    {
        select_digit(Digit);
    }
}
