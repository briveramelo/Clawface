using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformMemento {

    public Vector3 startPosition;
    public Vector3 startScale;
    public Quaternion startRotation;

    public void Initialize(Transform transform) {
        startPosition = transform.localPosition;
        startScale = transform.localScale;
        startRotation = transform.localRotation;
    }
    
}

namespace ModMan {
    public static class Transforms {

        public static void Reset(this Transform transform, TransformMemento tMemento){
            transform.localPosition = tMemento.startPosition;
            transform.localScale = tMemento.startScale;
            transform.localRotation = tMemento.startRotation;
        }

    }
}
