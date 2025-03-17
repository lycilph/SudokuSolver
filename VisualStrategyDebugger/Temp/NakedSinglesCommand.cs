namespace VisualStrategyDebugger.Temp;

public class NakedSinglesCommand : IGridCommand
{
    public string Description => "Naked Singles Command";

    public void Do()
    {
    }

    public void Undo()
    {
    }

    public IVisualizer GetVisualizer()
    {
        throw new NotImplementedException();
    }
}
