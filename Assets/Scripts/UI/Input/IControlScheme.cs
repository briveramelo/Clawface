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

    ButtonMode[] GetAction(List<IController> controllers, string action);
    bool GetAction(List<IController> controllers, string action, ButtonMode mode);
}
