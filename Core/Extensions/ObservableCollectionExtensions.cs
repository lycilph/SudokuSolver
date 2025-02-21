using System.Collections.ObjectModel;

namespace Core.Extensions;

public static class ObservableCollectionExtensions
{
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> list)
    {
        return new ObservableCollection<T>(list);
    }
}
