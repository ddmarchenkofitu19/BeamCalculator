namespace BeamCalculator.Helpers;

public static class ListExtensions
{
    public static IList<T> InsertAfter<T>(this IList<T> list, T itemToAdd, T previousItem)
    {
        var index = list.IndexOf(previousItem);
        list.Insert(index + 1, itemToAdd);
        return list;
    }

    public static IList<T> InsertBefore<T>(this IList<T> list, T itemToAdd, T nextItem)
    {
        var index = list.IndexOf(nextItem);
        list.Insert(index - 1, itemToAdd);
        return list;
    }
}
