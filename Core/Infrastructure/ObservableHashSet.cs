using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Core.Infrastructure;

public class ObservableHashSet<T> : ISet<T>, INotifyCollectionChanged, INotifyPropertyChanged
{
    private readonly HashSet<T> _hashSet;

    public ObservableHashSet()
    {
        _hashSet = new HashSet<T>();
    }

    public ObservableHashSet(IEnumerable<T> collection)
    {
        _hashSet = new HashSet<T>(collection);
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        CollectionChanged?.Invoke(this, e);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public int Count
    {
        get => _hashSet.Count;
    }

    public bool IsReadOnly => false;

    public bool Add(T item)
    {
        if (_hashSet.Add(item))
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            OnPropertyChanged(nameof(Count));
            return true;
        }
        return false;
    }

    public void Clear()
    {
        if (_hashSet.Count > 0)
        {
            _hashSet.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(nameof(Count));
        }
    }

    public bool Remove(T item)
    {
        if (_hashSet.Remove(item))
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            OnPropertyChanged(nameof(Count));
            return true;
        }
        return false;
    }

    public void UnionWith(IEnumerable<T> other)
    {
        var oldItems = new HashSet<T>(_hashSet);
        _hashSet.UnionWith(other);
        NotifySetChanges(oldItems, nameof(UnionWith));
    }

    public void IntersectWith(IEnumerable<T> other)
    {
        var oldItems = new HashSet<T>(_hashSet);
        _hashSet.IntersectWith(other);
        NotifySetChanges(oldItems, nameof(IntersectWith));
    }

    public void ExceptWith(IEnumerable<T> other)
    {
        var oldItems = new HashSet<T>(_hashSet);
        _hashSet.ExceptWith(other);
        NotifySetChanges(oldItems, nameof(ExceptWith));
    }

    // Other ISet<T> methods (pass-through to _hashSet)
    public bool Contains(T item) => _hashSet.Contains(item);
    public void CopyTo(T[] array, int arrayIndex) => _hashSet.CopyTo(array, arrayIndex);
    public IEnumerator<T> GetEnumerator() => _hashSet.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        var oldItems = new HashSet<T>(_hashSet);
        _hashSet.SymmetricExceptWith(other);
        NotifySetChanges(oldItems, nameof(SymmetricExceptWith));
    }
    public bool IsSubsetOf(IEnumerable<T> other) => _hashSet.IsSubsetOf(other);
    public bool IsSupersetOf(IEnumerable<T> other) => _hashSet.IsSupersetOf(other);
    public bool IsProperSupersetOf(IEnumerable<T> other) => _hashSet.IsProperSupersetOf(other);
    public bool IsProperSubsetOf(IEnumerable<T> other) => _hashSet.IsProperSubsetOf(other);
    public bool Overlaps(IEnumerable<T> other) => _hashSet.Overlaps(other);
    public bool SetEquals(IEnumerable<T> other) => _hashSet.SetEquals(other);

    void ICollection<T>.Add(T item) => Add(item); // Explicit interface implementation

    private void NotifySetChanges(HashSet<T> oldItems, string operationName)
    {
        var newItems = new HashSet<T>(_hashSet);
        if (!oldItems.SetEquals(newItems))
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(nameof(Count));
        }
    }
}
