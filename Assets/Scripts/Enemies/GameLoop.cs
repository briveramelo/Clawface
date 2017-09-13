using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    EnemySystem m_EnemySystem = null;

    // Use this for initialization
    void Awake ()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);

        m_EnemySystem = new EnemySystem();
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_EnemySystem.Update();
	}
}
