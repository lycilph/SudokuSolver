using CommunityToolkit.Mvvm.ComponentModel;
using Core.Infrastructure;
using SudokuUI.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SudokuUI.ViewModels;

public partial class SelectionViewModel : ObservableObject
{
    private readonly PuzzleService puzzle_service;
    private readonly SelectionService selection_service;
    private readonly HighlightService highlight_service;

    public ObservableCollection<DigitViewModel> Digits { get; private set; }

    public bool IsHintMode
    {
        get => selection_service.InputMode == SelectionService.Mode.Hints;
        set => selection_service.InputMode = (value ? SelectionService.Mode.Hints : SelectionService.Mode.Digits);
    }

    public SelectionViewModel(PuzzleService puzzle_service, SelectionService selection_service, HighlightService highlight_service)
    {
        this.puzzle_service = puzzle_service;
        this.selection_service = selection_service;
        this.highlight_service = highlight_service;

        Digits = Enumerable.Range(1, 9).Select(i => new DigitViewModel(i)).ToObservableCollection();

        selection_service.PropertyChanged += SelectionChanged;
        puzzle_service.GridValuesChanged += GridValuesChanged;

        UpdateMissingDigits();
    }

    private void GridValuesChanged(object? sender, EventArgs e)
    {
        UpdateMissingDigits();
    }

    private void SelectionChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectionService.InputMode))
        {
            OnPropertyChanged(nameof(IsHintMode));
        }
        else if (e.PropertyName == nameof(SelectionService.Digit))
        {
            highlight_service.HighlightNumber(selection_service.Digit);
        }
    }

    private void UpdateMissingDigits()
    {
        var digit_count = puzzle_service.CountDigits().ToArray();
        for (int i = 0; i < Digits.Count; i++)
            Digits[i].Missing = 9 - digit_count[i];
    }
}
