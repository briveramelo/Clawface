﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollGroupHelper : MonoBehaviour {

    #region Public Fields

    public static PLEUIItem currentUIItem = null;

    #endregion

    #region Private fields

    private static PointerEventData pointerData;
    private static UnityEngine.EventSystems.EventSystem current;
    private static GameObject selectedObject = null;
    private List<RaycastResult> results = new List<RaycastResult>();
    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        current = UnityEngine.EventSystems.EventSystem.current;
        pointerData = new PointerEventData(current);

    }

    private void Update()
    {
        RaycastToScrollGroup();
    }

    #endregion

    #region Private Interface

    private void RaycastToScrollGroup()
    {
        currentUIItem = null;
        pointerData.position = Input.mousePosition;

        results = new List<RaycastResult>();

        current.RaycastAll(pointerData, results);

        if (results.Count > 0)
        {
            foreach (RaycastResult r in results)
            {
                currentUIItem = r.gameObject.GetComponent<PLEUIItem>();

                if (currentUIItem)
                    break;
            }
        }
        
    }

    #endregion
}
