using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour {

    private void OnEnable()
    {
        Invoke("Die", 1f);
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }
}
