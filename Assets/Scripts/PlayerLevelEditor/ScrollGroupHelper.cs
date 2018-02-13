using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollGroupHelper : MonoBehaviour {

    #region Private fields

    private static PointerEventData pointerData;
    private static UnityEngine.EventSystems.EventSystem current;
    private static GameObject selectedProp = null;
    
    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        current = UnityEngine.EventSystems.EventSystem.current;
        pointerData = new PointerEventData(current);

    }

    public static GameObject RaycastToScrollGroup()
    {
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();

        current.RaycastAll(pointerData, results);

        if (results.Count > 0)
        {
            foreach (RaycastResult r in results)
            {
                PLEProp currentProp = r.gameObject.GetComponent<PLEProp>();
                if (currentProp)
                {
                    selectedProp = currentProp.registeredProp;
                }
            }
        }

        return selectedProp;
    }

    #endregion
}
