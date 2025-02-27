using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Extensions;
using Core.Model;
using SudokuUI.Controllers;
using System.Collections.ObjectModel;

namespace SudokuUI.ViewModels;

public partial class CellViewModel : ObservableObject
{
    private Cell cell;
    private SelectionController selection_controller;

    [ObservableProperty]
    private ObservableCollection<HintViewModel> hints;

    [ObservableProperty]
    private int value = 0;

    public CellViewModel(Cell cell, SelectionController selection_controller)
    {
        this.cell = cell;
        Value = cell.Value;

        hints = Enumerable.Range(1, 9).Select(i => new HintViewModel(cell.Candidates.Contains(i) ? i : 0)).ToObservableCollection();
        this.selection_controller = selection_controller;
    }

    [RelayCommand]
    private void CellClicked()
    {
        if (cell.IsFilled)
        {
            selection_controller.DigitSelected = cell.Value;
        }
        else
        {
            if (selection_controller.DigitSelected != 0)
            {
                if (selection_controller.InputMode == SelectionController.Mode.Digits)
                    Value = selection_controller.DigitSelected;
                else
                    ToggleHint(selection_controller.DigitSelected);
            }

        }
    }

    private void ToggleHint(int digit)
    {
        if (Hints[digit-1].Digit == 0)
            Hints[digit-1].Digit = digit;
        else
            Hints[digit-1].Digit = 0;
    }
}
