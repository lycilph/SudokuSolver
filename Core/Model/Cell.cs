using CommunityToolkit.Mvvm.ComponentModel;
using Core.Infrastructure;
using System.Diagnostics;

namespace Core.Model;

[DebuggerDisplay("Id = {Index}, ({Value}) [{GetCandidatesAsString()}]")]
public partial class Cell(int index) : ObservableObject
{
    public int Index { get; set; } = index;
    [ObservableProperty]
    private bool isClue = false;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFilled), nameof(IsEmpty))]
    private int value = 0;
    public ObservableHashSet<int> Candidates { get; set; } = [.. Grid.PossibleValues];
    public Cell[] Peers { get; set; } = [];

    public bool IsFilled => Value != 0;
    public bool IsEmpty => Value == 0;

    public int Row => Index / 9;
    public int Column => Index % 9;
    public int GetIndexInUnit(UnitType type) => type == UnitType.Row ? Row : Column;

    public int CandidatesCount() => Candidates.Count;
    public void AddAllCandidates() => Candidates.UnionWith([.. Grid.PossibleValues]);
    public bool Has(int candidate) => Candidates.Contains(candidate);
    public void Add(int candidate) => Candidates.Add(candidate);
    public void Remove(int candidate) => Candidates.Remove(candidate);

    public void Reset()
    {
        Value = 0;
        IsClue = false;
        AddAllCandidates();
    }

    public string GetCandidatesAsString() => string.Join("", Grid.PossibleValues.Select(v => Candidates.Contains(v) ? v.ToString() : "."));
}