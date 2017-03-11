using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enum Example
public enum Enemy_Type
{
    Mall_Cop,
    Enemy_type2,
    Enemy_type3,
}


public class EnemyFactory : IEnemyFactory
{
    GameObject m_Target = null;

    EnemyController m_controller;


    public EnemyFactory()
    {

    }

    public EnemyFactory(GameObject Player)
    {
        m_Target = Player;
        m_controller = new EnemyController();
    }
    

    public override void CreateEnemy(Enemy_Type i_type, GameObject m_GameObject)
    {
        IEnemy NewEnemy = null;

        switch(i_type)
        {
            case Enemy_Type.Mall_Cop:
                Debug.Log("Mall_Cop");
                Debug.Log(m_Target);
                NewEnemy = new LaiCop(m_Target);
                NewEnemy.SetGameObject(m_GameObject);
                NewEnemy.SetAI(new LaiMallCopAI(NewEnemy));

                 m_controller.AddEnemy(NewEnemy);
                break;


            default:
                Debug.Log("Cannot Create this Type");
                break;
        }

        return;
    }
}
