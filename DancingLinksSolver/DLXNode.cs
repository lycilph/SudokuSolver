namespace DancingLinksSolver;

public class DLXNode
{
    public DLXNode Left, Right, Up, Down;
    public ColumnNode Column = null!;
    public int RowID = -1;

    public DLXNode() { Left = Right = Up = Down = this; }
}
