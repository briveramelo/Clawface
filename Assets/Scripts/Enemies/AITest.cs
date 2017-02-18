using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITest : MonoBehaviour
{
    LaiCap lai = new LaiCap();

	// Use this for initialization
	void Start ()
    {
        lai.SetGameObject(gameObject);
        lai.SetAI(new EnemyAI(lai));
    }
	
	// Update is called once per frame
	void Update ()
    {
        lai.UpdateAI();
    }
}
