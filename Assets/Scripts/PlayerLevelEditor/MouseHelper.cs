using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;
public class MouseHelper : MonoBehaviour {


    #region Public Fields

    public static GameObject currentHoveredObject;
    public static PLEBlockUnit currentBlockUnit;
    public static PLESpawn currentSpawn;
    public static PLEProp currentProp;
    public static Vector3 currentMouseWorldPosition;
    public static RaycastHit? raycastHit;
    public static RaycastHit raycastHitTile;
    public static bool hitItem;
    public static bool hitTile;
    #endregion

    #region Private Fields

    private Ray r;
    private int groundMask, obstacleMask, groundOrObstacleMask;
    #endregion

    #region Unity Lifecycle


    private void Start() {
        groundMask = LayerMask.GetMask(Strings.Layers.GROUND);
        obstacleMask = LayerMask.GetMask(Strings.Layers.OBSTACLE);
        groundOrObstacleMask = groundMask | obstacleMask;
    }
    private void Update()
    {
        Vector3 mousePos = Input.mousePosition.NoZ();
        r = Camera.main.ScreenPointToRay(mousePos);
        
        RaycastItems(r);
        RaycastGround(r);
    }    

    void RaycastItems(Ray r) {
        RaycastHit[] hits = Physics.RaycastAll(r, 1000.0f);
        hitItem = hits.Length>0;
        float closestDistance = 10000f;
        raycastHit = null;
        
        for (int i = 0; i < hits.Length; i++) {
            RaycastHit hit = hits[i];
            string hitName = hit.transform.name;
            if (!hitName.Contains(Strings.BLOCKINGOBJECT) && !hitName.Contains("Wall")) {
                float distance = Vector3.Distance(hit.transform.position, Camera.main.transform.position);
                if (distance< closestDistance) {
                    closestDistance = distance;
                    raycastHit = hit;
                }
            }
        }
        if (hitItem) {
            if (raycastHit==null) {
                raycastHit = hits[0];
            }
            currentMouseWorldPosition = raycastHit.Value.transform.position;
            currentHoveredObject = raycastHit.Value.transform.gameObject;
            currentProp = currentHoveredObject.GetComponent<PLEProp>();
            currentSpawn = currentHoveredObject.GetComponent<PLESpawn>();            
        }
        else {
            currentHoveredObject = null;
            currentProp = null;
            currentSpawn = null;
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
