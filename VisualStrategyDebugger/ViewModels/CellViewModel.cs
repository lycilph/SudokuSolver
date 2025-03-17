using CommunityToolkit.Mvvm.ComponentModel;
using Core.Infrastructure;
using Core.Model;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace VisualStrategyDebugger.ViewModels;

public partial class CellViewModel : ObservableObject
{
    [ObservableProperty]
    private Cell cell;
    
    [ObservableProperty]
    private ObservableCollection<CandidateViewModel> candidates;

    [ObservableProperty]
    private bool highlight;

    public CellViewModel(Cell cell)
    {
        this.cell = cell;

        Candidates = Grid.PossibleValues
            .Select(i => new CandidateViewModel(cell, i) { IsVisible = cell.Has(i) })
            .ToObservableCollection();
        Highlight = false;

        cell.Candidates.CollectionChanged += CandidatesCollectionChanged;
    }

    private void CandidatesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        foreach (var candidate in Candidates)
            candidate.IsVisible = Cell.Has(candidate.Value);
    }
}
