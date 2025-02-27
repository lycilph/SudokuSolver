using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Model;
using SudokuUI.Controllers;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SudokuUI.ViewModels;

public partial class SelectionViewModel : ObservableObject
{
    private Grid grid;
    private SelectionController selection_controller;

    public ObservableCollection<DigitSelectionViewModel> DigitSelections { get; private set; }

    [ObservableProperty]
    private bool _inputtingHints = false;

    public SelectionViewModel(Grid grid, SelectionController selection_controller)
    {
        this.grid = grid;
        this.selection_controller = selection_controller;

        DigitSelections = [.. Enumerable.Range(1, 9).Select(d => new DigitSelectionViewModel(d, selection_controller))];

        selection_controller.PropertyChanged += SelectionChanged;

        RefreshFromModel();
    }

    partial void OnInputtingHintsChanged(bool value)
    {
        selection_controller.InputMode = InputtingHints ? SelectionController.Mode.Hints : SelectionController.Mode.Digits;
    }

    private void SelectionChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectionController.InputMode))
        {
            InputtingHints = selection_controller.InputMode == SelectionController.Mode.Hints;
        }
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
        selection_controller.ClearDigitSelection();
    }
}
