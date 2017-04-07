using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelEditor {

	Camera ActiveCamera { get; }

    Rect SelectionRect { get; }

    Ray PointerRay { get; }
}
