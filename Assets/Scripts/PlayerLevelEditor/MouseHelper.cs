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
    public static RaycastHit? raycastHitTile;
    public static bool HitItem { get; private set; }
    public static bool HitTile { get; private set; }
    public static bool HitUI { get { return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(); } }
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
        bool hitUI = HitUI;
        RaycastItems(r, hitUI);
        RaycastGround(r, hitUI);
    }    

    void RaycastItems(Ray r, bool hitUI) {
        RaycastHit[] hits = Physics.RaycastAll(r, 1000.0f);
        HitItem = hits.Length>0;
        if (hitUI) {
            HitItem = false;
        }
        System.Predicate<string> nameCheck = hitName=>!hitName.Contains(Strings.BLOCKINGOBJECT) && !hitName.Contains("Wall");
        raycastHit = HitItem ? GetClosestHit(hits, nameCheck) : null;

        
        if (HitItem) {
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

    void RaycastGround(Ray r, bool hitUI) {
        RaycastHit[] hits = Physics.RaycastAll(r, 1000.0f, groundOrObstacleMask);
        HitTile = hits.Length > 0;
        if (hitUI) {
            HitTile = false;
        }
        System.Predicate<string> nameCheck = hitName => hitName.Contains("RealBlock");
        raycastHitTile = HitTile ? GetClosestHit(hits, nameCheck) : null;

        if (HitTile) {
            if (raycastHitTile == null) {
                raycastHitTile = hits[0];
            }
            currentBlockUnit = raycastHitTile.Value.transform.GetComponent<PLEBlockUnit>();
            if (currentBlockUnit==null) {
                HitTile = false;
                raycastHitTile = null;
            }
        }
        else {
            currentBlockUnit = null;
        }
    }

    RaycastHit? GetClosestHit(RaycastHit[] hits, System.Predicate<string> hitNameChecks) {
        float closestDistance = 10000f;
        RaycastHit? thisHit = null;
        for (int i = 0; i < hits.Length; i++) {
            RaycastHit hit = hits[i];
            string hitName = hit.transform.name;
            
            if (hitNameChecks(hitName)) {
                float distance = Vector3.Distance(hit.transform.position, Camera.main.transform.position);
                if (distance < closestDistance) {
                    closestDistance = distance;
                    thisHit = hit;
                }
            }
        }
        return thisHit;
    }    

    #endregion 

}
