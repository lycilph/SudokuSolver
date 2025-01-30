using System.Diagnostics;

namespace Sandbox.Model;

public enum UnitType { Row, Column };

[DebuggerDisplay("I = {Index}, V = {Value}, Candidates = {CandidatesCount}")]
public class Cell(int index)
{
    public int Index { get; set; } = index;

    private int _value = 0;
    public int Value 
    { 
        get => _value; 
        set  
        { 
            _value = value;
            Candidates.Clear();
        }
    }

    public HashSet<int> Candidates { get; set; } = new HashSet<int>(Grid.PossibleValues);

    public Cell[] Peers { get; set; } = [];

    public bool HasValue => Value != 0;
    public bool IsEmpty => Value == 0;
    public int CandidatesCount => Candidates.Count;

    public int Row => Index / 9;
    public int Column => Index % 9;

    public int GetUnitIndex(UnitType type) => type == UnitType.Row ? Row : Column;
}
