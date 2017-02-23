using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnScript : MonoBehaviour {

    #region Serialized Unity Inspector fields
    [SerializeField]
    private float lockRadius;
    [SerializeField]
    private int rayCastRotationIncrement;
    [SerializeField]
    private int rayCastRotationRange;
    #endregion

    #region Private Fields
    private GameObject currentEnemy;
    private bool isTargetting;
    private bool isChangingTarget;
    private LayerMask enemyMask;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        enemyMask = LayerMask.GetMask(Strings.Tags.ENEMY);
        currentEnemy = null;        
        isTargetting = false;
        isChangingTarget = false;
    }
	
	// Update is called once per frame
	void Update () {
        CheckIfCurrentTargetIsAlive();
        CheckForInputsAndAcquireTarget();
    }
    #endregion

    #region Public Methods
    public GameObject GetCurrentEnemy()
    {
        return currentEnemy;
    }
    #endregion

    #region Private Methods

    void CheckIfCurrentTargetIsAlive()
    {
        if (currentEnemy != null && !currentEnemy.activeSelf)
        {
            currentEnemy = null;
        }
    }

    void CheckForInputsAndAcquireTarget()
    {
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.LOCK, ButtonMode.HELD))
        {
            if (!isTargetting)
            {
                isTargetting = true;
                AcquireTarget();
            }
            if (InputManager.Instance.QueryAxes(Strings.Input.Axes.LOOK).magnitude > 0.0F)
            {
                if (!isChangingTarget)
                {
                    isChangingTarget = true;
                    Vector2 aimAxis = InputManager.Instance.QueryAxes(Strings.Input.Axes.LOOK);
                    Vector3 inputDirection = Camera.main.transform.TransformDirection(new Vector3(aimAxis.x, 0.0f, aimAxis.y));
                    inputDirection.y = 0;
                    GetClosestEnemies(inputDirection);
                }
            }
            else
            {
                isChangingTarget = false;
            }
        }
        else if (InputManager.Instance.QueryAction(Strings.Input.Actions.LOCK, ButtonMode.UP))
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
            if (hit.transform.gameObject.tag == Strings.Tags.ENEMY)
            {
                float distance = Vector3.Distance(hit.transform.position, transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    currentEnemy = hit.transform.gameObject;
                }
            }
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
    }
    #endregion

}
