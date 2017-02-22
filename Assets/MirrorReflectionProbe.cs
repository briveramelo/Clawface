using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorReflectionProbe : MonoBehaviour {

	ReflectionProbe _reflectionProbe;

    private void Awake() {
        _reflectionProbe = GetComponent<ReflectionProbe>();
    }

    private void Update() {
        _reflectionProbe.transform.position = new Vector3 (
            Camera.main.transform.position.x,
            -Camera.main.transform.position.y,
            Camera.main.transform.position.z);

       _reflectionProbe.RenderProbe();
    }
}
