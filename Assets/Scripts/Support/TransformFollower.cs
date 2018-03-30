using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollower : MonoBehaviour {

    [SerializeField] private Transform other;
	void Update () {
        transform.position = other.position;
    }
}
