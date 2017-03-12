using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enum Example: add enemy type here (or in otherwhere)
public enum Enemy_Type
{
    Mall_Cop,
    Enemy_type2,
    Enemy_type3,
}


public class EnemyFactory : IEnemyFactory
{

    EnemySystem m_EnemySystem = new EnemySystem();


    public EnemyFactory()
    {

    }
    
    public override IEnemy CreateEnemy(GameObject m_GameObject, Enemy_Type i_type)
    {
        IEnemy NewEnemy = null;

        switch(i_type)
        {
            case Enemy_Type.Mall_Cop:

                NewEnemy = new LaiCop();
                NewEnemy.SetGameObject(m_GameObject);
                NewEnemy.SetAI(new LaiMallCopAI(NewEnemy));

                m_EnemySystem.AddEnemy(NewEnemy);
                break;

            default:
                Debug.Log("Cannot Create this Type");
                break;
        }

        return NewEnemy;
    }
}
