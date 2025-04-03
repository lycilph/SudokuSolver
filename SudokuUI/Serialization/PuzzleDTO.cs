using Core.Serialization;

namespace SudokuUI.Serialization;

public class PuzzleDTO
{
    public string Source { get; set; } = string.Empty;
    public List<CellDTO> Cells { get; set; } = [];

    PuzzleDTO() { }

    public PuzzleDTO(string source, List<CellDTO> cells)
    {
        Source = source;
        Cells = cells;
    }
}
