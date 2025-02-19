namespace Core.Model;

public class SolveActionElement
{
    public string Description { get; set; } = string.Empty;
    public int Number { get; set; } = 0;
    public List<Cell> Cells { get; set; } = [];
}
