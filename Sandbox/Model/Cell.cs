using System.Diagnostics;

namespace Sandbox.Model;

[DebuggerDisplay("I = {Index}, V = {Value}")]
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

    public HashSet<int> Candidates { get; set; } = new HashSet<int>(Enumerable.Range(1, 9));

    public Cell[] Peers { get; set; } = [];

    public bool HasValue => Value != 0;
    public bool IsEmpty => Value == 0;
    public int CandidatesCount => Candidates.Count;
}
