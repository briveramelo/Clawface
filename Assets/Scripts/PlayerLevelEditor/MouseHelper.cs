using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHelper : MonoBehaviour {


    #region Public Fields

    public static GameObject currentHoveredObject;
    public static PLEBlockUnit currentBlockUnit;
    public static PLESpawn currentSpawn;
    public static PLEProp currentProp;
    public static Vector3 currentMouseWorldPosition;
    public static RaycastHit raycastHit;
    public static RaycastHit raycastHitTile;
    public static bool hitItem;
    public static bool hitTile;
    #endregion

    #region Private Fields

    private Ray r;
    private int layerMask;
    #endregion

    #region Unity Lifecycle


    private void Start() {
        layerMask = LayerMask.GetMask(Strings.Layers.GROUND);
    }
    private void Update()
    {
        r = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastItems(r);
        RaycastGround(r);
    }

    void RaycastItems(Ray r)
    {
        hitItem = Physics.Raycast(r, out raycastHit, 1000.0f);

        if (hitItem)
        {
            currentMouseWorldPosition = raycastHit.transform.position;

            currentHoveredObject = raycastHit.transform.gameObject;
            currentProp = currentHoveredObject.GetComponent<PLEProp>();
            currentSpawn = currentHoveredObject.GetComponent<PLESpawn>();            
        }
        else
        {
            currentHoveredObject = null;
            currentProp = null;
            currentSpawn = null;
        }
    }

    void RaycastGround(Ray r) {
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
