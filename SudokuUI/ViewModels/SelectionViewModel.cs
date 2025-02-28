using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Model;
using SudokuUI.Services;
using System.Collections.ObjectModel;

namespace SudokuUI.ViewModels;

public partial class SelectionViewModel : ObservableObject
{
    private Grid grid;
    //private SelectionService selection_service;

    public ObservableCollection<DigitSelectionViewModel> DigitSelections { get; private set; }
    public SelectionService SelectionService { get; private set; }

    //[ObservableProperty]
    //private bool _inputtingHints = false;

    public SelectionViewModel(Grid grid, SelectionService selection_service)
    {
        this.grid = grid;
        SelectionService = selection_service;

        DigitSelections = [.. Enumerable.Range(1, 9).Select(d => new DigitSelectionViewModel(d, selection_service)), new DigitSelectionViewModel(0, selection_service)];

        //selection_service.PropertyChanged += SelectionChanged;

        RefreshFromModel();
    }

    //partial void OnInputtingHintsChanged(bool value)
    //{
    //    selection_service.InputMode = InputtingHints ? SelectionService.Mode.Hints : SelectionService.Mode.Digits;
    //}

    //private void SelectionChanged(object? sender, PropertyChangedEventArgs e)
    //{
    //    if (e.PropertyName == nameof(SelectionService.InputMode))
    //    {
    //        InputtingHints = selection_service.InputMode == SelectionService.Mode.Hints;
    //    }
    //}

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
