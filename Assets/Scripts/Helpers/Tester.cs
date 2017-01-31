using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour {

    [SerializeField] Stats myStats;
	// Use this for initialization
	IEnumerator Start () {
        yield return new WaitForSeconds(0.5f);
        myStats.Modify(StatType.Attack, 2f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
