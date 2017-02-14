using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtension {

	public static T PopFront<T> (this List<T> list) {
        T result = list[0];
        list.RemoveAt(0);
        return result;
    }
}
