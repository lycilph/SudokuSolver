using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace Core.Model;

[DebuggerDisplay("Id = {Index}, ({Value}) [{GetCandidatesAsString()}]")]
public partial class Cell(int index) : ObservableObject
{
    public int Index { get; set; } = index;
    [ObservableProperty]
    private bool isClue = false;
    [ObservableProperty]
    private int value = 0;
    public HashSet<int> Candidates { get; set; } = [.. Grid.PossibleValues];
    public Cell[] Peers { get; set; } = [];

    public bool IsFilled => Value != 0;
    public bool IsEmpty => Value == 0;

    public int Row => Index / 9;
    public int Column => Index % 9;
    public int GetIndexInUnit(UnitType type) => type == UnitType.Row ? Row : Column;

    public int CandidatesCount() => Candidates.Count;

    public void Reset()
    {
        Value = 0;
        IsClue = false;
        Candidates = [.. Grid.PossibleValues];
    }

    public string GetCandidatesAsString() => string.Join("", Grid.PossibleValues.Select(v => Candidates.Contains(v) ? v.ToString() : "."));
}