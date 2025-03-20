namespace VisualStrategyDebugger.Messages;

public class ResetMessage(bool new_grid = false)
{
    public bool NewGrid { get; } = new_grid;
}
