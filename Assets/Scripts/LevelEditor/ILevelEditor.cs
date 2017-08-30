// ILevelEditor.cs
// Author: Aaron

using UnityEngine;

/// <summary>
/// Interface for classes that can edit levels.
/// </summary>
public interface ILevelEditor 
{
    /// <summary>
    /// Returns the camera that the user is seeing through (read-only).
    /// </summary>
	Camera ActiveCamera { get; }

    /// <summary>
    /// Returns the current selection Rect used by the UI (read-only).
    /// </summary>
    Rect SelectionRect { get; }

    /// <summary>
    /// Returns the Ray cast by the pointer (read-only).
    /// </summary>
    Ray PointerRay { get; }
}
