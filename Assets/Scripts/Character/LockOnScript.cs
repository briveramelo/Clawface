using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnScript : MonoBehaviour {

    [SerializeField]
    private float lockRadius;
    [SerializeField]
    private int rayCastRotationIncrement;
    [SerializeField]
    private int rayCastRotationRange;
    private GameObject currentEnemy;
    private GameObject currentEnemyRight;
    private GameObject currentEnemyLeft;
    private bool isTargetting;
    LayerMask enemyMask = LayerMask.GetMask(Strings.ENEMY);

    // Use this for initialization
    void Start () {
        currentEnemy = null;
        currentEnemyRight = null;
        currentEnemyLeft = null;
        isTargetting = false;        
    }
	
	// Update is called once per frame
	void Update () {        
        if(Input.GetAxis(Strings.RIGHTTRIGGER) < -0.5f)
        {
            if (!isTargetting)
            {
                isTargetting = true;
                AcquireTarget();
            }
            if (Mathf.Abs(Input.GetAxis(Strings.AIMX)) >= 0.5f)
            {
                if(Input.GetAxis(Strings.AIMX) < 0)
                {
                    SwitchEnemy(true);
                }else if(Input.GetAxis(Strings.AIMX) > 0)
                {
                    SwitchEnemy(false);
                }
            }
        }else if(Input.GetAxis(Strings.RIGHTTRIGGER) > -0.5f)
        {
            isTargetting = false;
            currentEnemy = null;
            currentEnemyLeft = null;
            currentEnemyRight = null;
        }
	}

    void SwitchEnemy(bool left)
    {
        GetClosestEnemies();
        if (left && currentEnemyLeft != null)
        {
            currentEnemy = currentEnemyLeft;
        }
        else if(currentEnemyRight != null)
        {
            currentEnemy = currentEnemyRight;            
        }
        GetClosestEnemies();
    }

    void AcquireTarget()
    {
        RaycastHit[] hits;
        Ray sphereRay = new Ray(transform.position, transform.forward);        
        hits = Physics.SphereCastAll(sphereRay, lockRadius, 0f, enemyMask);
        float minDistance = Mathf.Infinity;
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.gameObject.tag == Strings.ENEMY)
            {
                float distance = Vector3.Distance(hit.transform.position, transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    currentEnemy = hit.transform.gameObject;
                }
            }
        }
        if(currentEnemy != null)
        {
            print("current " + currentEnemy.name);
        }
        GetClosestEnemies();
    }

    void GetClosestEnemies()
    {
        if(currentEnemy != null)
        {
            Vector3 lookAtVector = currentEnemy.transform.position - transform.position;
            int count = rayCastRotationRange / rayCastRotationIncrement;
            for(int i = 1; i <= count; i++)
            {                
                Vector3 castDirection = Quaternion.Euler(0,rayCastRotationIncrement*i,0) * lookAtVector;
                RaycastHit hit;
                Ray ray = new Ray(transform.position, castDirection);
                if(Physics.Raycast(ray, out hit, lockRadius, enemyMask))
                {
                    if(hit.transform.gameObject != currentEnemy)
                    {
                        currentEnemyRight = hit.transform.gameObject;
                        print("Right "+ currentEnemyRight.name);
                        break;
                    }
                }
            }
            for (int i = -1; i >= -count; i--)
            {
                Vector3 castDirection = Quaternion.Euler(0, rayCastRotationIncrement * i, 0) * lookAtVector;
                RaycastHit hit;
                Ray ray = new Ray(transform.position, castDirection);
                if (Physics.Raycast(ray, out hit, lockRadius, enemyMask))
                {
                    if (hit.transform.gameObject != currentEnemy)
                    {
                        currentEnemyLeft = hit.transform.gameObject;
                        print("Left "+ currentEnemyLeft.name);
                        break;
                    }
                }
            }
        }
    }

    public GameObject GetCurrentEnemy()
    {
        return currentEnemy;
    }
}
