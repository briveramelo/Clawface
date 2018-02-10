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
    public static bool hitTile;
    #endregion

    #region Private Fields

    private Ray r;
    private int layerMask;
    private RaycastHit raycastHitTile;
    #endregion

    #region Unity Lifecycle


    private void Start() {
        layerMask = LayerMask.GetMask(Strings.Layers.GROUND);
    }
    private void Update() {
        r = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        hitItem = Physics.Raycast(r, out raycastHit, 1000.0f);
        if (hitItem) {
            currentHoveredObject = raycastHit.transform.gameObject;
            currentMouseWorldPosition = raycastHit.transform.position;
        }
        else {
            currentHoveredObject = null;
        }

        hitTile = Physics.Raycast(r, out raycastHitTile, 1000.0f, layerMask);
        if (hitTile) {
            currentBlockUnit = raycastHitTile.transform.gameObject.GetComponent<PLEBlockUnit>();
        }
        else {
            currentBlockUnit = null;
        }
    }

    #endregion 

}
