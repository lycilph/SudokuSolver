using System.Collections.ObjectModel;
using Core.Models;

namespace Core.Extensions;

public static class IEnumerableExtensions
{
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> list)
    {
        return [.. list];
    }

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> col, Action<T> action)
    {
        foreach (var i in col)
            action(i);
        return col;
    }

    public static string ToIndexString(this IEnumerable<Cell> list)
    {
        return string.Join(',', list.Select(c => c.Index));
    }
}
