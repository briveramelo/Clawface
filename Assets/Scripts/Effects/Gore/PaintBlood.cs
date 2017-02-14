using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBlood : MonoBehaviour {

    [SerializeField]
    private Texture2D paintDecal;

    private int minSplashes = 5;
    private int maxSplahses = 15;
    private float splashRange = 2f;

    private float minScale = 0.25f;
    private float maxScale = 2.5f;

    //debug
    private bool drawDebug = true;
    private Vector3 hitPointDebug;
    private List<Ray> rayDebugs = new List<Ray>();


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            

            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit h;

            if(Physics.Raycast(r, out h, Mathf.Infinity))
            {
                Debug.Log("PAINT");
            }
        }
    }
}
