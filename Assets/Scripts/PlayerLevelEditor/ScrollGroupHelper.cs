using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollGroupHelper : MonoBehaviour {

    #region Public Fields

    public static PLEProp currentSelectedProp;
    public static PLESpawn currentSelectedSpawn;

    #endregion

    #region Private fields

    private PointerEventData pointerData;
    private UnityEngine.EventSystems.EventSystem current;
    private List<RaycastResult> castedObjects = new List<RaycastResult>();

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        current = UnityEngine.EventSystems.EventSystem.current;
        pointerData = new PointerEventData(current);

    }

    private void Update()
    {
        current.RaycastAll(pointerData, castedObjects);

        if(castedObjects.Count > 0)
        {
            foreach(RaycastResult r in castedObjects)
            {

                currentSelectedProp = r.gameObject.GetComponent<PLEProp>();
                currentSelectedSpawn = r.gameObject.GetComponent<PLESpawn>();
            }
        }
    }

    #endregion
}
