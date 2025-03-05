using System.Collections.ObjectModel;

namespace Core.Infrastructure;

public static class ObservableCollectionsExtensions
{
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> list)
    {
        return new ObservableCollection<T>(list);
    }
}
