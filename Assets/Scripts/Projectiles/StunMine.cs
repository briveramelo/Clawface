using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunMine : MonoBehaviour {

    [SerializeField]
    private float damage;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == Strings.ENEMY)
        {
            other.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
            gameObject.SetActive(false);
        }
    }
}
