using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Extensions;
using Core.Models;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class DigitSelectionViewModel : ObservableObject
{
    private readonly SelectionService selection_service;

    public bool IsHintMode
    {
        get => selection_service.InputMode == SelectionService.Mode.Hints;
        set => selection_service.InputMode = (value ? SelectionService.Mode.Hints : SelectionService.Mode.Digits);
    }

    public ObservableCollection<DigitViewModel> Digits { get; private set; }

    public DigitSelectionViewModel(PuzzleService puzzle_service, SelectionService selection_service)
    {
        this.selection_service = selection_service;

        Digits = Grid.PossibleValues.Select(i => new DigitViewModel(i, selection_service)).ToObservableCollection();

        // Update the view first time
        UpdateSelectedDigit(selection_service.Digit);
        UpdateMissingDigits(puzzle_service.DigitCount());

        // Listen to changes
        puzzle_service.ValuesChanged += (s, e) => UpdateMissingDigits(e.DigitCount);
        selection_service.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SelectionService.InputMode))
                OnPropertyChanged(nameof(IsHintMode));
            else if (e.PropertyName == nameof(SelectionService.Digit))
                UpdateSelectedDigit(selection_service.Digit);
        };
    }

    private void UpdateSelectedDigit(int digit)
    {
        if (digit > 0)
        {
            var digit_to_select = Digits[digit - 1];
            if (digit_to_select.Missing <= 0 && Digits.Any(d => d.Missing > 0))
            {
                selection_service.Continue();
                return;
            }
        }

        Digits.ForEach(d => d.Selected = d.Digit == digit);
    }

    private void UpdateMissingDigits(List<int> digit_count)
    {
        Digits.ForEach(d => d.Missing = 9 - digit_count[d.Digit - 1]);
    }
}
