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
    void Start () {
        Assert.IsNotNull(start);
        Assert.IsNotNull(arm);
        Assert.IsNotNull(end);
        Assert.IsNotNull(clawAnimationHandler);
        meshSizeZ = arm.GetComponent<MeshRenderer>().bounds.size.z;
        clawAnimationHandler.gameObject.SetActive(false);
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
    public void StartExtension(GameObject target, float clawExtensionTime)
    {        
        this.target = target;
        extending = true;
        clawAnimationHandler.gameObject.SetActive(true);
        //clawAnimationHandler.StartAnimation();
        extendTime = clawExtensionTime;
    }

    public void StartRetraction(float clawRetractionTime)
    {
        extending = false;
        retracting = true;
        retractTime = clawRetractionTime;
    }

    public void ResetClawArm()
    {
        clawAnimationHandler.gameObject.SetActive(false);
        target = null;
        extending = false;
        retracting = false;
    }
    #endregion

    #region Private functions
    private void ResizeArm()
    {
        arm.transform.position = (start.position + end.position) / 2f;
        float zScale = Vector3.Distance(start.position, end.position) / meshSizeZ;
        arm.transform.localScale = new Vector3(1.0f, 1.0f, zScale);
        arm.transform.forward = (end.position - start.position).normalized;
    }

    private void ExtendArm()
    {
        if (!target.activeSelf)
        {
            EventSystem.Instance.TriggerEvent(Strings.Events.ARM_EXTENDED, end);
        }
        else if(Vector3.Distance(end.position, target.transform.position) <= 0.1f)
        {
            end.position = target.transform.position;
            EventSystem.Instance.TriggerEvent(Strings.Events.ARM_EXTENDED, end);
            //clawAnimationHandler.FinishAnimation();
        }
        else
        {
            Vector3 direction = (target.transform.position - end.position).normalized;
            end.position += direction * extendSpeed * Time.deltaTime;
            extendTime -= Time.deltaTime;
            direction = (target.transform.position - end.position).normalized;
            if (Vector3.Dot(direction, end.forward) <= 0 || extendTime <= 0)
            {
                end.position = target.transform.position;
            }
        }
    }

    private void RetractArm()
    {        
        if(Vector3.Distance(end.position, start.position) <= 0.1f)
        {
            end.position = start.position;
            EventSystem.Instance.TriggerEvent(Strings.Events.ARM_ANIMATION_COMPLETE);
        }
        else
        {
            Vector3 direction = (start.position - end.position).normalized;
            end.position += direction * retractSpeed * Time.deltaTime;
            retractTime -= Time.deltaTime;
            direction = (start.position - end.position).normalized;
            if (Vector3.Dot(direction, end.forward) >= 0 || retractTime <= 0)
            {
                end.position = start.position;
            }
        }
    }
    #endregion

}