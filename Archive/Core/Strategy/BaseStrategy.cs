﻿using Core.Model;
using Core.Model.Actions;

namespace Core.Strategy;

public abstract class BaseStrategy<T> : IStrategy where T : BaseStrategy<T>, IStrategy, new()
{
    private static readonly Lazy<T> _instance = new Lazy<T>(() => new T());

    public static T Instance => _instance.Value;

    public abstract string Name { get; }

    public abstract IPuzzleAction? Execute(Grid grid);

    public static IPuzzleAction? ExecuteAndApply(Grid grid)
    {
        var actions = Instance.Execute(grid);
        actions?.Do();
        return actions;
    }
}