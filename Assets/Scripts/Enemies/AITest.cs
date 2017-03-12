using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITest : MonoBehaviour
{
    public GameObject player;
    public GameObject Mallcop;

    IEnemyFactory factory;

    void Start()
    {
        factory = GameFactory.GetEnemyFactory();
    }


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Z))
        {
            Vector3 position = Vector3.zero;
            GameObject newObj = Instantiate(Mallcop, new Vector3(0, 0 , 0), Quaternion.identity) as GameObject;

            IEnemy newEnemy = factory.CreateEnemy(newObj, Enemy_Type.Mall_Cop);
            newEnemy.SetTarget(player);
        }
    }
}
