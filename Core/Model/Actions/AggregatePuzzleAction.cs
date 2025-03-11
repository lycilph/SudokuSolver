using Core.Infrastructure;

namespace Core.Model.Actions;

public class AggregatePuzzleAction : IPuzzleAction
{
    public readonly List<IPuzzleAction> actions = [];

    public void Add(IPuzzleAction action) => actions.Add(action);

    public void Do()
    {
        actions.ForEach(a => a.Do());
    }

    public void Undo()
    {
        actions.AsEnumerable().Reverse().ForEach(a => a.Undo());
    }
}
