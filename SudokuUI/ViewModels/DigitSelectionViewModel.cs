using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SudokuUI.Controllers;
using System.ComponentModel;

namespace SudokuUI.ViewModels;

public partial class DigitSelectionViewModel : ObservableObject
{
    private SelectionController selection_controller;

    [ObservableProperty]
    private int _digit;

    // The # of this digit left in the puzzle (ie. how many is left to place)
    [ObservableProperty]
    private int _missing = 9;

    [ObservableProperty]
    private bool _selected = false;

    public DigitSelectionViewModel(int digit, SelectionController selection_controller)
    {
        Digit = digit;
        this.selection_controller = selection_controller;

        selection_controller.PropertyChanged += SelectionChanged;
    }

    private void SelectionChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectionController.DigitSelected))
        {
            Selected = selection_controller.DigitSelected == Digit;
        }
    }

    [RelayCommand]
    private void Select()
    {
        selection_controller.DigitSelected = Digit;
    }
}
