using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITest : MonoBehaviour
{
    public GameObject player;
    LaiCop lai;


	// Use this for initialization
	void Start ()
    {
        lai = new LaiCop(player);
        lai.SetGameObject(gameObject);
        lai.SetAI(new EnemyAI(lai));
    }
	
	// Update is called once per frame
	void Update ()
    {
        lai.UpdateAI();
    }
}
