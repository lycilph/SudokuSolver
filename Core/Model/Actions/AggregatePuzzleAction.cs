using Core.Infrastructure;

namespace Core.Model.Actions;

/// <summary>
/// This action can aggregate puzzle and execute them 1 at a time. This is used
/// when multiple actions needs to the done/undone at once
/// </summary>
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
