using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Model;
using SudokuUI.Services;
using System.Collections.ObjectModel;

namespace SudokuUI.ViewModels;

public partial class SelectionViewModel : ObservableObject
{
    private Grid grid;

    public ObservableCollection<DigitSelectionViewModel> DigitSelections { get; private set; }
    public SelectionService SelectionService { get; private set; }

    public SelectionViewModel(Grid grid, SelectionService selection_service)
    {
        this.grid = grid;
        SelectionService = selection_service;

        DigitSelections = [.. Enumerable.Range(1, 9).Select(d => new DigitSelectionViewModel(d, selection_service)), new DigitSelectionViewModel(0, selection_service)];

        RefreshFromModel();
    }

    public void RefreshFromModel()
    {
        foreach (var digit in Grid.PossibleValues)
        {
            var cells_with_digit = grid.Cells.Count(c => c.Value == digit);
            DigitSelections[digit - 1].Missing = 9 - cells_with_digit;
        }
    }

    [RelayCommand]
    private void ClearSelection()
    {
        SelectionService.ClearDigitSelection();
    }
}
