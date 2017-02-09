using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGizmo : MonoBehaviour {

    [SerializeField, Range(0.2f,5f)] float radius;
    [SerializeField] Color color;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, radius);
    }
}
