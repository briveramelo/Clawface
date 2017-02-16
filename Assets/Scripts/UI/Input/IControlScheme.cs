/**
 *  @author Cornelia Schultz
 */

using System.Collections.Generic;
using UnityEngine;

public interface IControlScheme  {

    string SchemeName
    {
        get;
    }

    Vector2 GetAxes(List<IController> controllers, string axes);

    bool GetButton(List<IController> controllers, string button, ButtonMode mode);
}
