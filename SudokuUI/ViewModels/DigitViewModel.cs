using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class DigitViewModel : ObservableObject
{
    private readonly SelectionService selection_service;

    [ObservableProperty]
    private int digit;

    // The # of this digit left in the puzzle (ie. how many is left to place)
    [ObservableProperty]
    private int missing = 9;

    [ObservableProperty]
    private bool selected = false;

    public DigitViewModel(int digit, SelectionService selection_service)
    {
        this.selection_service = selection_service;

        Digit = digit;
    }

    [RelayCommand]
    private void Select()
    {
        selection_service.Digit = Selected ? Digit : 0;
    }
}
