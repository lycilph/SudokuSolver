using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Infrastructure;
using Core.Model;
using NLog;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace SudokuUI.ViewModels;

public partial class CellViewModel : ObservableObject
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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

        cell.PropertyChanged += CellPropertyChanged;
        cell.Candidates.CollectionChanged += CandidatesChanged;
    }

    private void CandidatesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Logger.Debug($"Cell {cell.Index} candidates changed: {e.Action}");
        foreach (var candidate in Candidates)
        {
            candidate.IsVisible = cell.HasCandidate(candidate.Value);
        }
    }

    private void CellPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Logger.Debug($"Cell {cell.Index} property changed: {e.PropertyName}");
        OnPropertyChanged(e.PropertyName);
    }

    [RelayCommand]
    private void SetValue()
    {
        Value = cell.Candidates.First();
    }

    [RelayCommand]
    private void SetCandidate()
    {
        cell.AddCandidate(5);
    }
}
