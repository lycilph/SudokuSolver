using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;

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

    public HashSet<int> Candidates { get; set; } = [];
    
    public Cell[] Peers { get; set; } = [];

    // Derived properties
    public bool IsFilled => Value != 0;
    public bool IsEmpty => Value == 0;

    // Candidate methods
    public bool Contains(int value) => Candidates.Contains(value);

    public string GetCandidatesAsString() => string.Join("", Grid.PossibleValues.Select(v => Candidates.Contains(v) ? v.ToString() : "."));

    public void Reset()
    {
        IsClue = false;
        Value = 0;
        Candidates.Clear();
    }
}
