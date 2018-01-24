using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ClawArmController : MonoBehaviour {

    #region serialized fields
    [SerializeField]
    private Transform start;
    [SerializeField]
    private GameObject arm;
    [SerializeField]
    private Transform end;
    [SerializeField]
    private ClawAnimationHandler clawAnimationHandler;    
    #endregion

    #region private fields
    private float meshSizeZ;
    private GameObject target;
    private bool extending;
    private bool retracting;
    private float extendTime;
    private float retractTime;
    private BoxCollider collider;

    private float extendSpeed {
        get
        {
            return Vector3.Distance(end.position, target.transform.position) / extendTime;
        }
    }

    private float retractSpeed {
        get
        {
            return Vector3.Distance(end.position, start.position) / retractTime;
        }
    }
    #endregion

    #region unity lifecycle    
    // Use this for initialization
    void Awake () {
        Assert.IsNotNull(start);
        Assert.IsNotNull(arm);
        Assert.IsNotNull(end);
        Assert.IsNotNull(clawAnimationHandler);
        meshSizeZ = arm.GetComponent<MeshRenderer>().bounds.size.z;
    }

    private void Start()
    {
        clawAnimationHandler.gameObject.SetActive(false);
        collider = GetComponent<BoxCollider>();
        if (collider)
        {
            collider.enabled = false;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        Assert.IsFalse(extending && retracting);

        if (extending)
        {
            ExtendArm();
        }

        if (retracting)
        {
            RetractArm();
        }

        ResizeArm();
    }
    #endregion

    #region Public functions
    public void StartExtension(GameObject target, float clawExtensionTime, float clawRetractionTime)
    {        
        this.target = target;
        extending = true;
        clawAnimationHandler.gameObject.SetActive(true);
        extendTime = clawExtensionTime;
        retractTime = clawRetractionTime;        
    }    

    public void ResetClawArm()
    {
        clawAnimationHandler.gameObject.SetActive(false);
        target = null;
        extending = false;
        retracting = false;
        end.position = start.position;
        end.forward = start.forward;
        if (collider)
        {
            collider.enabled = false;
            collider.center = Vector3.zero;
        }
    }

    public void ExtendClawToDistance(float radius, bool enableCollider = false)
    {
        clawAnimationHandler.gameObject.SetActive(true);
        end.position = start.position + start.forward * radius;
        if (collider)
        {
            collider.enabled = enableCollider;
        }
    }

    public Vector3 GetEndPosition()
    {
        return end.position;
    }
    #endregion

    #region Private functions
    private void StartRetraction()
    {
        extending = false;
        retracting = true;
    }

    private void ResizeArm()
    {
        arm.transform.position = (start.position + end.position) / 2f;
        float zScale = Vector3.Distance(start.position, end.position) / meshSizeZ;
        arm.transform.localScale = new Vector3(1.0f, 1.0f, zScale);
        if (end.position != start.position)
        {
            arm.transform.forward = (end.position - start.position).normalized;
        }

        if (collider)
        {
            collider.center = end.localPosition;
        }
    }

    private void ExtendArm()
    {
        if (!target.activeSelf)
        {
            EventSystem.Instance.TriggerEvent(Strings.Events.CAPTURE_ENEMY, end);
        }
        else
        {

            end.LookAt(target.transform.position);
            Vector3 direction = (target.transform.position - end.position).normalized;
            float distanceToTarget = Vector3.Distance(start.position, target.transform.position);
            end.position += direction * extendSpeed * Time.deltaTime;
            end.forward = direction;

            extendTime -= Time.deltaTime;
            direction = (target.transform.position - end.position).normalized;

            if (Vector3.Dot(direction, end.forward) <= 0 || extendTime <= 0)
            {
                end.position = target.transform.position;
                EventSystem.Instance.TriggerEvent(Strings.Events.CAPTURE_ENEMY, end);
                StartRetraction();
            }
        }
    }

    private void RetractArm()
    {   
        Vector3 direction = (start.position - end.position).normalized;
        end.position += direction * retractSpeed * Time.deltaTime;
        target.transform.position = end.position;
        retractTime -= Time.deltaTime;
        direction = (start.position - end.position).normalized;
        if (Vector3.Dot(direction, end.forward) >= 0 || retractTime <= 0)
        {
            end.position = start.position;
            EventSystem.Instance.TriggerEvent(Strings.Events.ARM_ANIMATION_COMPLETE);
        }
    }
    #endregion

}