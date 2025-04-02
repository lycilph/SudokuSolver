using Core.Models;

namespace Core.Serialization;

public class CellDTO
{
    public int Index { get; set; } = 0;
    public int Value { get; set; } = 0;
    public bool IsClue { get; set; } = false;
    public List<int> Candidates { get; set; } = [];

    public CellDTO() { }

    public CellDTO(Cell cell)
    {
        Index = cell.Index;
        Value = cell.Value;
        IsClue = cell.IsClue;
        Candidates = cell.Candidates.ToList();
    }
}
