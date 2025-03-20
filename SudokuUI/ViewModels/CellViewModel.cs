using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Extensions;
using Core.Models;
using NLog;

namespace SudokuUI.ViewModels;

public partial class CellViewModel : ObservableObject
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    [ObservableProperty]
    private Cell wrappedObject;

    [ObservableProperty]
    private ObservableCollection<CandidateViewModel> candidates;

    public CellViewModel(Cell cell)
    {
        WrappedObject = cell;

        Candidates = Grid.PossibleValues
            .Select(i => new CandidateViewModel(cell, i) { IsVisible = cell.Contains(i) })
            .ToObservableCollection();
    }
}
