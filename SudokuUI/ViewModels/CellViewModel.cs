using System.Collections.ObjectModel;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Extensions;
using Core.Models;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using ObservableCollections;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class CellViewModel : ObservableObject
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private readonly PuzzleService puzzle_service;
    private readonly SelectionService selection_service;
    private readonly Brush error_highlight_color;
    private readonly Brush default_highlight_color;
    private readonly Brush default_background_color;

    [ObservableProperty]
    private Cell wrappedObject;

    [ObservableProperty]
    private ObservableCollection<CandidateViewModel> candidates;

    [ObservableProperty]
    private bool highlight = false;

    [ObservableProperty]
    private Brush highlightColor = Brushes.Black;

    [ObservableProperty]
    private Brush backgroundColor = Brushes.Black;

    public CellViewModel(Cell cell)
    {
        puzzle_service = App.Current.Services.GetRequiredService<PuzzleService>();
        selection_service = App.Current.Services.GetRequiredService<SelectionService>();
        error_highlight_color = App.Current.Resources["cell_error_color"] as Brush ?? Brushes.Black;
        default_highlight_color = App.Current.Resources["cell_highlight_color"] as Brush ?? Brushes.Black;
        default_background_color = App.Current.Resources["cell_background_color"] as Brush ?? Brushes.Black;

        WrappedObject = cell;

        Candidates = Grid.PossibleValues
            .Select(i => new CandidateViewModel(cell, i) { IsVisible = cell.Contains(i) })
            .ToObservableCollection();

        WrappedObject.Candidates.CollectionChanged += CandidatesChanged;

        ResetVisuals();
    }

    private void CandidatesChanged(in NotifyCollectionChangedEventArgs<int> e)
    {
        foreach (var candidate in Candidates)
            candidate.IsVisible = WrappedObject.Contains(candidate.Value);
    }

    public void Set()
    {
        if (WrappedObject.IsClue)
        {
            selection_service.Digit = WrappedObject.Value;
            return;
        }

        if (selection_service.Digit == 0)
        {
            logger.Info("Nothing to set");
            return;
        }

        var digit = selection_service.Digit;

        if (selection_service.InputMode == SelectionService.Mode.Digits)
            puzzle_service.SetCellValue(WrappedObject, digit);

        if (selection_service.InputMode == SelectionService.Mode.Hints && WrappedObject.IsEmpty)
            puzzle_service.ToggleCellCandidate(WrappedObject, digit);
    }

    [RelayCommand]
    private void Clear()
    {
        if (WrappedObject.IsClue)
            return;

        puzzle_service.ClearCell(WrappedObject);
    }

    public void ResetVisuals()
    {
        Highlight = false;
        HighlightColor = default_highlight_color;
        BackgroundColor = default_background_color;
    }

    internal void MarkError()
    {
        Highlight = true;
        HighlightColor = error_highlight_color;
    }
}
