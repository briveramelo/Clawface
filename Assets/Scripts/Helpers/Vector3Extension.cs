// Vector3Extension.cs

using UnityEngine;

/// <summary>
/// Extension class for Unity Vector3s.
/// </summary>
public static class Vector3Extension {

    /// <summary>
    /// Returns the greatest of this vector's components.
    /// </summary>
	public static float Max (this Vector3 v) {
        return Mathf.Max (v.x, v.y, v.z);
    }
}
