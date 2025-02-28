using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Extensions;
using Core.Model;
using SudokuUI.Services;
using System.Collections.ObjectModel;

namespace SudokuUI.ViewModels;

public partial class CellViewModel : ObservableObject
{
    private Cell cell;
    private SelectionService selection_service;

    [ObservableProperty]
    private ObservableCollection<HintViewModel> hints;

    [ObservableProperty]
    private int value = 0;

    public CellViewModel(Cell cell, SelectionService selection_service)
    {
        this.cell = cell;
        Value = cell.Value;

        hints = Enumerable.Range(1, 9).Select(i => new HintViewModel(cell.Candidates.Contains(i) ? i : 0)).ToObservableCollection();
        this.selection_service = selection_service;
    }

    [RelayCommand]
    private void CellClicked()
    {
        if (cell.IsFilled)
        {
            selection_service.Digit = cell.Value;
        }
        else
        {
            if (selection_service.Digit != -1)
            {
                if (selection_service.InputMode == SelectionService.Mode.Digits)
                    Value = selection_service.Digit;
                else
                    ToggleHint(selection_service.Digit);
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
