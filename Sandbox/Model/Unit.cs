namespace Sandbox.Model;

public class Unit
{
    public string Name { get; set; }

    public int Index { get; set; }

    public Cell[] Cells { get; set; } = [];
}
