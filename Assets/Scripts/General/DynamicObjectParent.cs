﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicObjectParent : MonoBehaviour {

    public static DynamicObjectParent instance;

	// Use this for initialization
	void Start () {
		if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
	}

}
