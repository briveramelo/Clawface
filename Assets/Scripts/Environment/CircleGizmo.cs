using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DrawMode {
    Always=0,
    OnSelected=1
}

public class CircleGizmo : MonoBehaviour {

    [SerializeField] DrawMode drawMode;
	[SerializeField] Transform circleEnd;
    [SerializeField] Color color;

    private void OnDrawGizmosSelected()
    {
        if (drawMode == DrawMode.OnSelected) {
            DrawGizmo();
        }
    }

    private void OnDrawGizmos() {
        if (drawMode==DrawMode.Always) {
            DrawGizmo();
        }
    }

    void DrawGizmo() {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, Vector3.Distance(transform.position, circleEnd.position));
    }
}
