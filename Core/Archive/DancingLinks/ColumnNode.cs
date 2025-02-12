namespace Core.Archive.DancingLinks;

public class ColumnNode : DLXNode
{
    public int Size; // Number of 1s in the column
    public string Name;

    public ColumnNode(string name)
    {
        Name = name;
        Size = 0;
        Column = this;
    }
}
