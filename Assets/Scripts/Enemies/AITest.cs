using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITest : MonoBehaviour
{
    public GameObject player;

    public GameObject Mallcop;

    EnemyController controller;

    IEnemyFactory factory;


    void Start()
    {
        controller = new EnemyController();
        factory = GameFactory.GetEnemyFactory(player);
    }


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Z))
        {
            Vector3 position = Vector3.zero;
            GameObject newObj = Instantiate(Mallcop, new Vector3(0, 0 , 0), Quaternion.identity) as GameObject;
            factory.CreateEnemy(Enemy_Type.Mall_Cop, newObj);
        }

        controller.Update();

    }


    /*
    IEnemy lai;

	// Use this for initialization
	void Start ()
    {
        lai = new LaiCop(player);
        lai.SetGameObject(gameObject);
        lai.SetAI(new LaiMallCopAI(lai));
    }
	
	// Update is called once per frame
	void Update ()
    {
        lai.UpdateAI();
    }

    */

}
