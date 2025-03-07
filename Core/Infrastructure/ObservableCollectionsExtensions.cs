using System.Collections.ObjectModel;

namespace Core.Infrastructure;

public static class ObservableCollectionsExtensions
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
}
