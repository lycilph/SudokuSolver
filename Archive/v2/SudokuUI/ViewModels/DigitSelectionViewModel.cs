using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SudokuUI.Services;
using System.ComponentModel;

namespace SudokuUI.ViewModels;

public partial class DigitSelectionViewModel : ObservableObject
{
    private SelectionService selection_service;

    [ObservableProperty]
    private int _digit;

    // The # of this digit left in the puzzle (ie. how many is left to place)
    [ObservableProperty]
    private int _missing = 9;

    [ObservableProperty]
    private bool _selected = false;

    public DigitSelectionViewModel(int digit, SelectionService selection_service)
    {
        Digit = digit;
        this.selection_service = selection_service;

        selection_service.PropertyChanged += SelectionChanged;
    }

    private void SelectionChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectionService.Digit))
        {
            Selected = selection_service.Digit == Digit;
        }
    }

    [RelayCommand]
    private void Select()
    {
        selection_service.Digit = Digit;
    }
}
