namespace Core.Models;

public class Link(int value, Cell start, Cell end)
{
    public int Value { get; set; } = value;

    public Cell Start { get; set; } = start;
    public Cell End { get; set; } = end;
    public IEnumerable<Cell> Cells { get; private set; } = [start, end];

    public Unit StartBox { get; set; } = null!;
    public Unit EndBox { get; set; } = null!;
    public IEnumerable<Unit> Boxes { get; private set; } = [];

    public void SetBoxes(Unit start, Unit end)
    {
        StartBox = start;
        EndBox = end;
        Boxes = [start, end];
    }

    public Unit? OverlapsInBox(Link other) => Boxes.Intersect(other.Boxes).FirstOrDefault();
    public Cell? OverlapsInCells(Link other) => Cells.Intersect(other.Cells).FirstOrDefault();

    public Cell GetEnd(Unit box) => box.Cells.Contains(Start) ? Start : End;
    public Cell GetOtherEnd(Unit box) => box.Cells.Contains(Start) ? End : Start;

    public override string ToString() => $"({Start.Index},{End.Index})";
}
