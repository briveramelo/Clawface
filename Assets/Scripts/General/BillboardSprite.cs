using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    new Camera camera;

    void Start ()
    {
        StartCoroutine (BillboardCoroutine());
    }

    IEnumerator BillboardCoroutine ()
    {
        while (true)
        {
            if (camera == null)
                camera = Camera.main;

            yield return null;

            if (camera != null)
                transform.LookAt (camera.transform, Vector3. up);

            yield return null;
        }
    }
}
