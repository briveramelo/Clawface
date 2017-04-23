using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ModMan;

public class LockOnScript : MonoBehaviour {

    #region Serialized Unity Inspector fields
    [SerializeField]
    private float lockRadius;
    [SerializeField]
    private int rayCastRotationIncrement;
    [SerializeField]
    private int rayCastRotationRange;
    [SerializeField] private Transform rayCastOrigin;
    #endregion

    #region Private Fields
    private GameObject currentEnemy;
    private bool isTargetting;
    private bool isChangingTarget;
    private int enemyMask;
    private Vector3 raycastDirection;
    #endregion

    #region Unity Lifecycle
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(rayCastOrigin.position, raycastDirection);
    }

    // Use this for initialization
    void Start () {                
        enemyMask = LayerMasker.GetLayerMask(Layers.Enemy);
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
        if (InputManager.Instance.QueryAction(Strings.Input.Actions.LOCK, ButtonMode.DOWN))
        {
            if (!isTargetting)
            {
                isTargetting = true;
                AcquireTarget();
            }
            else {
                isTargetting = false;
                currentEnemy = null;
            }
            
        }
        if (isTargetting) {
            CheckToChangeTarget();
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

    private void CheckToChangeTarget() {
        if (InputManager.Instance.QueryAxes(Strings.Input.Axes.LOOK).magnitude > 0.0F)
        {
            if (!isChangingTarget)
            {
                isChangingTarget = true;
                Vector2 aimAxis = InputManager.Instance.QueryAxes(Strings.Input.Axes.LOOK);
                Vector3 inputDirection = Camera.main.transform.TransformDirection(new Vector3(aimAxis.x, 0.0f, aimAxis.y));//new Vector3(aimAxis.x, 0, aimAxis.y);// transform.forward;
                inputDirection.y = 0;
                GetClosestEnemies(inputDirection);
            }
        }
        else
        {
            isChangingTarget = false;
        }
    }

    void GetClosestEnemies(Vector3 direction)
    {
        GameObject currentEnemyRight = null;
        GameObject currentEnemyLeft = null;
        if (currentEnemy != null)
        {            
            int count = rayCastRotationRange / rayCastRotationIncrement;
            float smallestAngleDifference = 1000f;
            for(int i = 1; i <= count; i++)
            {
                SetCurrentEnemies(i, direction, ref currentEnemyRight, ref smallestAngleDifference);
            }
            for (int i = -1; i >= -count; i--)
            {
                SetCurrentEnemies(i, direction, ref currentEnemyLeft, ref smallestAngleDifference);
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

    Ray ray = new Ray();
    RaycastHit[] hits = new RaycastHit[10];
    private void SetCurrentEnemies(int i, Vector3 inputDirection, ref GameObject currentEnemySide, ref float smallestAngleDifference) {
        raycastDirection = Quaternion.Euler(0, rayCastRotationIncrement * i, 0) * inputDirection;
        ray.origin = rayCastOrigin.position;
        ray.direction = raycastDirection;
        
        Physics.RaycastNonAlloc(ray, hits, rayCastRotationRange);
        foreach (RaycastHit hit in hits) {
            if (hit.transform!=null && hit.transform.gameObject != currentEnemy && hit.transform.gameObject.CompareTag(Strings.Tags.ENEMY)){
                Vector3 pointDir = -(transform.position - hit.point).NormalizedNoY();
                float angleDiff = Mathf.Abs(inputDirection.As360Angle() - pointDir.As360Angle());
                if (angleDiff < smallestAngleDifference) {
                    smallestAngleDifference = angleDiff;
                    currentEnemySide = hit.transform.gameObject;
                }                    
            }                                
        }
    }

    
    #endregion

}
