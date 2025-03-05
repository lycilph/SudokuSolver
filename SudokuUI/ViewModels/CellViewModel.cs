using CommunityToolkit.Mvvm.ComponentModel;
using Core.Infrastructure;
using Core.Model;
using System.Collections.ObjectModel;

namespace SudokuUI.ViewModels;

public partial class CellViewModel : ObservableObject
{
    private readonly Cell cell;

    public int Value
    {
        get => cell.Value;
        set => cell.Value = value;
    }

    public bool IsClue
    {
        get => cell.IsClue;
        set => cell.IsClue = value;
    }

    public bool IsFilled { get => cell.IsFilled; }

    [ObservableProperty]
    private ObservableCollection<CandidateViewModel> candidates;

    public CellViewModel(Cell cell)
    {
        this.cell = cell;

        Candidates = Grid.PossibleValues
            .Select(i => new CandidateViewModel(i) { IsVisible = cell.HasCandidate(i) })
            .ToObservableCollection();
    }
}
