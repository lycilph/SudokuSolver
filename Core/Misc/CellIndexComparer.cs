﻿using Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace Core.Misc;

public class CellIndexComparer : IEqualityComparer<Cell>
{
    public static CellIndexComparer Instance { get; } = new CellIndexComparer();

    public bool Equals(Cell? x, Cell? y)
    {
        return x?.Index == y?.Index;
    }

    public int GetHashCode([DisallowNull] Cell obj)
    {
        return obj.Index.GetHashCode();
    }
}
