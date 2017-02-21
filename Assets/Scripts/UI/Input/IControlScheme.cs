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

    Vector2 QueryAxes(List<IController> controllers, string axes);

    ButtonMode[] QueryAction(List<IController> controllers, string action);
    bool QueryAction(List<IController> controllers, string action, ButtonMode mode);
}
