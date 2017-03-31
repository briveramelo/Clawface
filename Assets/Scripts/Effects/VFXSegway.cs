using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXSegway : MonoBehaviour {

    [SerializeField] private GameObject idleParticles;
    [SerializeField] private GameObject moveParticles;

    public void SetMoving(bool isMoving) {
        moveParticles.SetActive(isMoving);
    }

    public void SetIdle(bool isMoving) {
        idleParticles.SetActive(isMoving);
    }
}
