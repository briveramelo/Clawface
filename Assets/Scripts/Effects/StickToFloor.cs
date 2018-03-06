using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToFloor : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] float offset = 0.01f;
    [SerializeField] Vector3 lastHit;
    [SerializeField] float rayStartOffset = 10.0f;

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast (new Ray(transform.parent.position + new Vector3(0.0f, rayStartOffset, 0.0f), Vector3.down), out hit, Mathf.Infinity, layerMask))
        {
            Vector3 position = transform.position;
            position.y = hit.point.y + offset;
            transform.position = position;
            lastHit = hit.point;
        }
        else lastHit = transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine (transform.position + new Vector3(0.0f, rayStartOffset, 0.0f), transform.position + Vector3.down * 20.0f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere (transform.position, 1.0f);
        Gizmos.DrawLine(transform.position, lastHit);
        Gizmos.DrawWireSphere (lastHit, 1.0f);
    }

    public float DistanceFromParent
    {
        get
        {
            return Vector3.Distance (transform.parent.position, transform.position);
        }
    }
}
