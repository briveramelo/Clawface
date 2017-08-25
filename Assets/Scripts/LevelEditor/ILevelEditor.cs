// ILevelEditor.cs

using UnityEngine;

/// <summary>
/// Interface for classes that can edit levels.
/// </summary>
public interface ILevelEditor 
{
	Camera ActiveCamera { get; }

    Rect SelectionRect { get; }

    Ray PointerRay { get; }
}
