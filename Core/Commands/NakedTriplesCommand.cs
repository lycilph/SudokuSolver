namespace Core.Commands;

/// <summary>
/// See NakedTriplesStrategy for more information
/// </summary>

public class NakedTriplesCommand(string name) : BaseCommand(name)
{
    public override void Do()
    {
        foreach (var element in Elements)
            foreach (var cell in element.Cells)
                cell.Candidates.Remove(element.Number);
    }

    public override void Undo()
    {
        foreach (var element in Elements)
            foreach (var cell in element.Cells)
                cell.Candidates.Add(element.Number);
    }
}
