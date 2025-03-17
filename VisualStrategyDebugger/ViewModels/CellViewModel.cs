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
        if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            if (e.OldItems != null) 
            {
                foreach (var candidate in e.OldItems.Cast<int>())
                {
                    Candidates[candidate-1].IsVisible = false;
                }
            }
        }
        else
            throw new NotImplementedException();
    }
}
