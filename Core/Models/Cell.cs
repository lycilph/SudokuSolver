using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using ObservableCollections;

namespace Core.Models;

[DebuggerDisplay("Id = {Index}, ({Value}) [{GetCandidatesAsString()}]")]
public partial class Cell(int index) : ObservableObject
{
    public int Index { get; set; } = index;

    [ObservableProperty]
    private bool isClue = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFilled), nameof(IsEmpty))]
    private int value = 0;

    public ObservableHashSet<int> Candidates { get; set; } = [];

    public Cell[] Peers { get; set; } = [];

    // Indices (ie. the cells index in a row or column)
    public int Row => Index / 9;
    public int Column => Index % 9;
    public int GetIndexInUnit(UnitType type) => type == UnitType.Row ? Row : Column;

    // Derived properties
    public bool IsFilled => Value != 0;
    public bool IsEmpty => Value == 0;

    // Candidate methods
    public int Count() => Candidates.Count;
    public bool Contains(int value) => Candidates.Contains(value);
    public void Add(int value) => Candidates.Add(value);
    public void Remove(int value) => Candidates.Remove(value);
    public void FillCandidates() => Candidates.AddRange(Grid.PossibleValues);
    public void Clear() => Candidates.Clear();
    public void Toggle(int value)
    {
        if (Contains(value))
            Remove(value);
        else
            Add(value);
    }

    public string GetCandidatesAsString() => string.Join("", Grid.PossibleValues.Select(v => Candidates.Contains(v) ? v.ToString() : "."));

    public void Reset()
    {
        IsClue = false;
        Value = 0;
        Candidates.Clear();
    }
}
