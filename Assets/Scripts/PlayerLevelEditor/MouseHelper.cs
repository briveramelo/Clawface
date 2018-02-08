using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHelper : MonoBehaviour {


    #region Public Fields

    public GameObject currentHoveredObject;
    public PLEBlockUnit currentBlockUnit;
    public Vector3 currentMouseWorldPosition;

    #endregion

    #region Private Fields

    private Ray r;
    private RaycastHit h;

    #endregion

    #region Unity Lifecycle

    private void Update()
    {
        r = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(r, out h, 1000.0f))
        {
            currentHoveredObject = h.transform.gameObject;
            currentBlockUnit = currentHoveredObject.GetComponent<PLEBlockUnit>();
            currentMouseWorldPosition = h.transform.position;

        }
    }

    #endregion 

}
