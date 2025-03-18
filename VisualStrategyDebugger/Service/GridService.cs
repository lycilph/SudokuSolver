using Core.Model;

namespace VisualStrategyDebugger.Service;

public class GridService
{
    private string source = ".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4";

    public Grid Grid { get; private set; } = new Grid();

    public void Reset()
    {
        Grid.Set(source);
    }

    public void Import(string input)
    {
        source = input;
        Reset();
    }
}
