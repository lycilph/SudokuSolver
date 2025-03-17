namespace VisualStrategyDebugger.Temp;

public interface IGridCommand
{
    public string Name { get; }
    public string Description { get; }

    public void Do();
    public void Undo();

    public void UpdateDescription(string name);

    public IVisualizer GetVisualizer();
}
