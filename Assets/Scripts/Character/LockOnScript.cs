using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnScript : MonoBehaviour {

    [SerializeField]
    private float lockRadius;
    [SerializeField]
    private float targetAngle;
    private GameObject closestEnemy;
    private GameObject closestEnemyRight;
    private GameObject closestEnemyLeft;
    private bool isTargetting;

    // Use this for initialization
    void Start () {
        closestEnemy = null;
        closestEnemyRight = null;
        closestEnemyLeft = null;
        isTargetting = false;        
    }
	
	// Update is called once per frame
	void Update () {        
        if(Input.GetAxis(Strings.RIGHTTRIGGER) < -0.5f && !isTargetting)
        {
            isTargetting = true;
            AcquireTarget();
        }else if(Input.GetAxis(Strings.RIGHTTRIGGER) > -0.5f)
        {
            isTargetting = false;
            closestEnemy = null;
        }
	}

    void AcquireTarget()
    {
        RaycastHit[] hits;
        Ray sphereRay = new Ray(transform.position, transform.forward);
        LayerMask mask = LayerMask.GetMask("Enemy");
        hits = Physics.SphereCastAll(sphereRay, lockRadius, 0f, mask);
        float distance = Mathf.Infinity;
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.gameObject.tag == Strings.ENEMY)
            {
                if(hit.distance < distance)
                {
                    if (Vector3.Angle(new Vector3(transform.forward.x, 0, transform.forward.z), new Vector3(hit.transform.position.x, 0, hit.transform.position.z)) < targetAngle) {
                        closestEnemy = hit.transform.gameObject;
                    }
                }
            }
        }
    }

    public GameObject GetCurrentEnemy()
    {
        return closestEnemy;
    }
}
