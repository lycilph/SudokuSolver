using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Infrastructure;
using Core.Model;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using SudokuUI.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace SudokuUI.ViewModels;

public partial class CellViewModel : ObservableObject
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private readonly PuzzleService puzzle_service;
    private readonly SelectionService selection_service;

    [ObservableProperty]
    private Cell _cell;

    [ObservableProperty]
    private ObservableCollection<CandidateViewModel> candidates;

    [ObservableProperty]
    private bool highlight = false;

    public CellViewModel(Cell cell)
    {
        Cell = cell;

        puzzle_service = App.Current.Services.GetRequiredService<PuzzleService>();
        selection_service = App.Current.Services.GetRequiredService<SelectionService>();

        Candidates = Grid.PossibleValues
            .Select(i => new CandidateViewModel(cell, i) { IsVisible = cell.Has(i) })
            .ToObservableCollection();

        Cell.Candidates.CollectionChanged += CandidatesChanged;
    }

    private void CandidatesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        logger.Trace("Cell {0} candidates changed: {1}", Cell.Index, e.Action);
        foreach (var candidate in Candidates)
        {
            candidate.IsVisible = Cell.Has(candidate.Value);
        }
    }

    [RelayCommand]
    private void SetDigit()
    {
        if (Cell.IsClue)
        {
            selection_service.Digit = Cell.Value;
            return;
        }

        if (selection_service.Digit == 0)
        {
            logger.Info("Nothing to set");
            return;
        }

        if (selection_service.InputMode == SelectionService.Mode.Digits)
            puzzle_service.SetValue(Cell, selection_service.Digit);
        
        if (selection_service.InputMode == SelectionService.Mode.Hints && Cell.IsEmpty)
            puzzle_service.ToggleCandidate(Cell, selection_service.Digit);
    }

    [RelayCommand]
    private void ClearDigit()
    {
        if (Cell.IsClue)
            return;

        puzzle_service.ClearCell(Cell);
    }
}
