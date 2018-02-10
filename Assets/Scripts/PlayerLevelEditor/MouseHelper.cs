using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHelper : MonoBehaviour {


    #region Public Fields

    public static GameObject currentHoveredObject;
    public static PLEBlockUnit currentBlockUnit;
    public static Vector3 currentMouseWorldPosition;
    public static RaycastHit raycastHit;
    public static bool hitItem;
    #endregion

    #region Private Fields

    private Ray r;

    #endregion

    #region Unity Lifecycle

    private void Update()
    {
        r = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
        hitItem = Physics.Raycast(r, out raycastHit, 1000.0f);
        if (hitItem) {
            currentHoveredObject = raycastHit.transform.gameObject;
            currentBlockUnit = currentHoveredObject.GetComponent<PLEBlockUnit>();
            currentMouseWorldPosition = raycastHit.transform.position;
        }
    }

    #endregion 

}
