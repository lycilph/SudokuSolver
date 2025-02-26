using System.Collections.ObjectModel;

namespace SudokuUI.Extensions;

public static class ObservableCollectionExtensions
{
    public static ObservableCollection<T> ForEach<T>(this ObservableCollection<T> collection, Action<T> action)
    {
        foreach (var item in collection)
            action(item);

        return collection;
    }
}
