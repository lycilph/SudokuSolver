namespace Core.Extensions;

public static class ListExtensions
{
    public static IEnumerable<(T, T)> GetPairCombinations<T>(this List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = i + 1; j < list.Count; j++)
            {
                yield return (list[i], list[j]);
            }
        }
    }
}
