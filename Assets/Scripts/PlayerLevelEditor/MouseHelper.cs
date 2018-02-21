using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;
public class MouseHelper : MonoBehaviour {


    #region Public Fields

    public static GameObject currentHoveredObject;
    public static PLEBlockUnit currentBlockUnit;
    public static Vector3 currentMouseWorldPosition;
    public static RaycastHit raycastHit;
    public static RaycastHit raycastHitTile;
    public static bool hitItem;
    public static bool hitTile;
    #endregion

    #region Private Fields

    private Ray r;
    private int groundMask, ignoreObstacleMask;
    #endregion

    #region Unity Lifecycle


    private void Start() {
        groundMask = LayerMask.GetMask(Strings.Layers.GROUND);
        ignoreObstacleMask = ~LayerMask.GetMask(Strings.Layers.OBSTACLE);
    }
    private void Update()
    {
        Vector3 mousePos = Input.mousePosition.NoZ();
        r = Camera.main.ScreenPointToRay(mousePos);
        
        RaycastItems(r);
        RaycastGround(r);
    }    

    void RaycastItems(Ray r)
    {
        hitItem = Physics.Raycast(r, out raycastHit, 1000.0f, ignoreObstacleMask);

        if (hitItem) {
            currentHoveredObject = raycastHit.transform.gameObject;
            currentMouseWorldPosition = raycastHit.point;
        }
        else {
            currentHoveredObject = null;
        }
    }

    void RaycastGround(Ray r) {
        hitTile = Physics.Raycast(r, out raycastHitTile, 1000.0f, groundMask);
        if (hitTile) {
            currentBlockUnit = raycastHitTile.transform.gameObject.GetComponent<PLEBlockUnit>();
        }
        else {
            currentBlockUnit = null;
        }
    }

    #endregion 

}
