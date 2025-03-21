using System.Collections.ObjectModel;
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

    private readonly SelectionService selection_service;

    [ObservableProperty]
    private Cell wrappedObject;

    [ObservableProperty]
    private ObservableCollection<CandidateViewModel> candidates;

    [ObservableProperty]
    private bool highlight = false;

    public CellViewModel(Cell cell)
    {
        selection_service = App.Current.Services.GetRequiredService<SelectionService>();

        WrappedObject = cell;

        Candidates = Grid.PossibleValues
            .Select(i => new CandidateViewModel(cell, i) { IsVisible = cell.Contains(i) })
            .ToObservableCollection();

        WrappedObject.Candidates.CollectionChanged += CandidatesChanged;
    }

    private void CandidatesChanged(in NotifyCollectionChangedEventArgs<int> e)
    {
        foreach (var candidate in Candidates)
            candidate.IsVisible = WrappedObject.Contains(candidate.Value);
    }

    [RelayCommand]
    private void Set()
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
            WrappedObject.Value = digit;

        if (selection_service.InputMode == SelectionService.Mode.Hints && WrappedObject.IsEmpty)
            WrappedObject.Toggle(digit);
    }
}
