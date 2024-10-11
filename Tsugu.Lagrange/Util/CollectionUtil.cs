namespace Tsugu.Lagrange.Util;

public static class CollectionUtil {
    public static void AddAll<TElement>(this ICollection<TElement> collection, ICollection<TElement> elements) {
        foreach (TElement element in elements) {
            collection.Add(element);
        }
    }
}
