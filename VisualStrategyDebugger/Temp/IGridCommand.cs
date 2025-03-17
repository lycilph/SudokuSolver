namespace VisualStrategyDebugger.Temp;

public interface IGridCommand
{
    public string Description { get; }

    public void Do();
    public void Undo();
}
