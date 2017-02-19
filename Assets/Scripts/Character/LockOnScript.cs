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
    private bool isTargetting;
    private bool isChangingTarget;
    LayerMask enemyMask = LayerMask.GetMask(Strings.ENEMY);

    // Use this for initialization
    void Start () {
        currentEnemy = null;        
        isTargetting = false;
        isChangingTarget = false;
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
            if (Mathf.Abs(Input.GetAxis(Strings.AIMX)) > 0.0f || Mathf.Abs(Input.GetAxis(Strings.AIMY)) > 0.0f)
            {
                if (!isChangingTarget)
                {
                    isChangingTarget = true;
                    Vector3 inputDirection = Camera.main.transform.TransformDirection(new Vector3(Input.GetAxis(Strings.AIMX), 0.0f, Input.GetAxis(Strings.AIMY)));
                    inputDirection.y = 0;
                    GetClosestEnemies(inputDirection);
                }
            }else
            {
                isChangingTarget = false;
            }
        }else if(Input.GetAxis(Strings.RIGHTTRIGGER) > -0.5f)
        {
            isTargetting = false;
            currentEnemy = null;
        }
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
    }

    void GetClosestEnemies(Vector3 direction)
    {
        GameObject currentEnemyRight = null;
        GameObject currentEnemyLeft = null;
        if (currentEnemy != null)
        {            
            int count = rayCastRotationRange / rayCastRotationIncrement;
            for(int i = 1; i <= count; i++)
            {                
                Vector3 castDirection = Quaternion.Euler(0,rayCastRotationIncrement*i,0) * direction;
                RaycastHit hit;
                Ray ray = new Ray(transform.position, castDirection);
                Debug.DrawRay(transform.position, castDirection, Color.red, 1f);
                if(Physics.Raycast(ray, out hit, Mathf.Infinity, enemyMask))
                {
                    if(hit.transform.gameObject != currentEnemy)
                    {
                        currentEnemyRight = hit.transform.gameObject;
                        break;
                    }
                }
            }
            for (int i = -1; i >= -count; i--)
            {
                Vector3 castDirection = Quaternion.Euler(0, rayCastRotationIncrement * i, 0) * direction;
                RaycastHit hit;
                Ray ray = new Ray(transform.position, castDirection);
                Debug.DrawRay(transform.position, castDirection, Color.green, 1f);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, enemyMask))
                {
                    if (hit.transform.gameObject != currentEnemy)
                    {
                        currentEnemyLeft = hit.transform.gameObject;
                        break;
                    }
                }
            }
        }
        if(currentEnemyLeft != null && currentEnemyRight != null)
        {
            if(Vector3.Distance(currentEnemyLeft.transform.position, transform.position) > Vector3.Distance(currentEnemyRight.transform.position, transform.position))
            {
                currentEnemy = currentEnemyRight;
            }
            else
            {
                currentEnemy = currentEnemyLeft;
            }
        }else if(currentEnemyLeft != null)
        {
            currentEnemy = currentEnemyLeft;
        }
        else if(currentEnemyRight != null)
        {
            currentEnemy = currentEnemyRight;
        }
        print("current " + currentEnemy.name);
    }

    public GameObject GetCurrentEnemy()
    {
        return currentEnemy;
    }
}
