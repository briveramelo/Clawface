// ListExtension.cs

using System.Collections.Generic;

/// <summary>
/// Extension class for lists.
/// </summary>
public static class ListExtension {

    /// <summary>
    /// Removes and returns the element at the front (0) of a list.
    /// </summary>
	public static T PopFront<T> (this List<T> list) {
        T result = list[0];
        list.RemoveAt(0);
        return result;
    }
}
